using System.Collections;
using System.Collections.Generic;
using org.COPASI;
using System.Diagnostics;
using System;
using CellUnity.Reaction;

namespace CellUnity.Simulation.Copasi
{
	public class Copasi : IDisposable
	{
		public Copasi ()
		{
			InitCopasi ();

			// create a new datamodel
			dataModel = CCopasiRootContainer.addDatamodel();
			
			// get the model from the datamodel
			model = dataModel.getModel();
			
			// set the units for the model
			// we want seconds as the time unit
			// nanoliter as the volume units
			// and nanomole as the substance units
			model.setTimeUnit(CUnit.s);
			model.setVolumeUnit(CUnit.nl);
			model.setQuantityUnit(CUnit.nMol);
			
			// create a compartment with the name cell
			compartment = model.createCompartment("cell"); // TODO: remove static value

			changedObjects = new ObjectStdVector();
		}

		public void Dispose()
		{
			CCopasiRootContainer.removeDatamodel (dataModel);
		}

		private static bool initCopasiCalled = false;
		private static void InitCopasi()
		{
			if (!initCopasiCalled)
			{
				// TODO: error handling
				CCopasiRootContainer.init();
				System.Diagnostics.Debug.Assert(CCopasiRootContainer.getRoot() != null);
				
				initCopasiCalled = true;
			}
		}

		private CCopasiDataModel dataModel;
		public CCopasiDataModel DataModel { get { return dataModel; } }

		private CModel model;
		public CModel Model { get { return model; } }

		private CTrajectoryTask trajectoryTask;
		public CTrajectoryTask TrajectoryTask  { get { return trajectoryTask; } }

		private CCompartment compartment;
		public CCompartment Compartment { get { return compartment; } }

		private ObjectStdVector changedObjects;

		public void InitTrajectoryTask()
		{
			TaskVectorN taskList = dataModel.getTaskList();
			
			// get the trajectory task object
			trajectoryTask = (CTrajectoryTask)taskList.getByName("Time-Course");
			
			// if there isn't one
			if (trajectoryTask == null)
			{
				// create a new one
				trajectoryTask = new CTrajectoryTask();
				
				// remove any existing trajectory task just to be sure since in
				// theory only the cast might have failed above
				taskList.removeByName("Time-Course");
				
				// add the new time course task to the task list
				taskList.add(trajectoryTask, true);
			}
			
			// run a deterministic time course
			trajectoryTask.setMethodType(CCopasiMethod.deterministic);
			
			// get the problem for the task to set some parameters
			CTrajectoryProblem problem = (CTrajectoryProblem)trajectoryTask.getProblem();
			
			// pass a pointer of the model to the problem
			problem.setModel(dataModel.getModel());
			
			// we want the model to be updated for each simulation
			trajectoryTask.setUpdateModel(true);
			
			// simulate only one step 
			problem.setStepNumber(1);
			
			model.setInitialTime(0.0);
			
			// set some parameters for the LSODA method through the method
			CTrajectoryMethod method = (CTrajectoryMethod)trajectoryTask.getMethod();
			
			CCopasiParameter parameter = method.getParameter("Absolute Tolerance");
			parameter.setDblValue(1.0e-12);
		}

		private Dictionary<MoleculeSpecies, CMetab> copasiMetabBySpecies = new Dictionary<MoleculeSpecies, CMetab>();
		private Dictionary<ReactionType, CReaction> copasiReactionByReactionType = new Dictionary<ReactionType, CReaction> ();
		private Dictionary<CReaction, CModelValue> copasiReactionFluxValueByReaction = new Dictionary<CReaction, CModelValue> ();

		public void UpdateCompartmentVolume(double volume)
		{
			compartment.setInitialValue (volume);
			CCopasiObject obj = compartment.getInitialValueReference();
			changedObjects.Add(obj);
		}

		public CMetab AddSpecies(MoleculeSpecies species)
		{
			CCompartment compartment = this.compartment;
			int status = CMetab.REACTIONS;

			CMetab metab = model.createMetabolite(species.Name, compartment.getObjectName());
			metab.setStatus(status);
		
			copasiMetabBySpecies.Add (species, metab);

			return metab;
		}

		public CMetab GetMetab(MoleculeSpecies species)
		{
			return copasiMetabBySpecies [species];
		}

		public void RemoveSpecies(MoleculeSpecies species)
		{
			model.removeMetabolite (GetMetab (species));
			copasiMetabBySpecies.Remove (species);
		}

		public void UpdateSpeciesQuantity(CMetab metab, double quantity)
		{
			metab.setInitialValue(quantity); // use setInitialValue for Number (not Concentration) see Doc 1.5.2.3 metabolites 
			CCopasiObject obj = metab.getInitialValueReference();
			changedObjects.Add(obj);
		}

		private CMetab[] GetMetabs(MoleculeSpecies[] species)
		{
			CMetab[] result = new CMetab[species.Length];
			
			for (int i = 0; i < species.Length; i++)
			{
				result[i] = GetMetab(species[i]);
			}
			
			return result;
		}

		public CReaction AddReaction(ReactionType reactionType)
		{
			string name = reactionType.GetAutoName();
			
			// now we create a reaction
			CReaction reaction = model.createReaction(name);
			
			UpdateReaction (reaction, reactionType.Reagents, reactionType.Products, reactionType.Rate);

			copasiReactionByReactionType.Add (reactionType, reaction);

			return reaction;
		}

		public void UpdateReaction(CReaction reaction, MoleculeSpecies[] reagents, MoleculeSpecies[] products, double rate)
		{
			// we can set these on the chemical equation of the reaction
			CChemEq chemEq = reaction.getChemEq();

			// remove all existing metabolites
			chemEq.getSubstrates ().clear ();
			chemEq.getProducts ().clear ();

			// add substrates
			CMetab[] substrates = GetMetabs(reagents);
			foreach (CMetab item in substrates)
			{
				// add substrate with stoichiometry 1
				chemEq.addMetabolite(item.getKey(), 1.0, CChemEq.SUBSTRATE);
			}

			// add products
			CMetab[] metabProducts = GetMetabs(products);
			foreach (CMetab item in metabProducts)
			{
				// add product with stoichiometry 1
				chemEq.addMetabolite(item.getKey(), 1.0, CChemEq.PRODUCT);
			}
			
			// this reaction is to be irreversible
			reaction.setReversible(false);
			
			
			// now we ned to set a kinetic law on the reaction
			// maybe constant flux would be OK
			// we need to get the function from the function database
			CFunctionDB funDB = CCopasiRootContainer.getFunctionList();
			
			// it should be in the list of suitable functions
			// lets get all suitable functions for an irreversible reaction with  x substrates
			// and y products
			CFunctionStdVector suitableFunctions = funDB.suitableFunctions((uint)substrates.Length, (uint)products.Length, COPASI.TriFalse);
			
			CFunction function = null;
			
			for (int i = 0; i < suitableFunctions.Count; i++)
			{
				// we just assume that the only suitable function with mass action in
				// it's name is the one we want
				if (suitableFunctions[i].getObjectName().ToLower().Contains("mass action"))
				{
					function = suitableFunctions[i];
					break;
				}
			}
			
			if (function != null)
			{
				reaction.setFunction(function);
				
				CCopasiParameterGroup parameterGroup = reaction.getParameters();
				CCopasiParameter parameter = parameterGroup.getParameter(0);
				// make sure the parameter is a local parameter
				System.Diagnostics.Debug.Assert(reaction.isLocalParameter(parameter.getObjectName()));
				// now we set the value of the parameter to 0.5
				System.Diagnostics.Debug.Assert(parameter.getType() == CCopasiParameter.DOUBLE);
				parameter.setDblValue(rate);
				CCopasiObject obj = parameter.getValueReference();
				changedObjects.Add(obj);

				//reaction.getParameterMappings().Clear();
				
				foreach (var substrate in substrates)
				{
					reaction.addParameterMapping("substrate", substrate.getKey());   
				}
			}
			else
			{
				throw new System.Exception("Error. Could not find a kinetic law that conatins the term \"Constant\".");
			}
		}

		public CReaction GetReaction(ReactionType reaction)
		{
			return copasiReactionByReactionType [reaction];
		}
		
		public void RemoveReaction(ReactionType reaction)
		{
			CReaction copasiReaction = GetReaction (reaction);
			model.removeReaction (copasiReaction);
			copasiReactionByReactionType.Remove (reaction);

			CModelValue fluxValue;
			if (copasiReactionFluxValueByReaction.TryGetValue (copasiReaction, out fluxValue))
			{
				model.removeModelValue(fluxValue);
				copasiReactionFluxValueByReaction.Remove(copasiReaction);
			}
		}

		public CModelValue AddReactionParticleFluxValue(CReaction reaction)
		{
			// create global quantity value
			
			CModelValue modelValue = model.createModelValue(reaction.getObjectName());
			
			modelValue.setInitialValue(0);
			var obj = modelValue.getInitialValueReference();
			changedObjects.Add(obj);
			
			// set the status to assignment
			modelValue.setStatus(CModelValue.ODE);
			// the assignment does not have to make sense
			string expression = "<CN=" + modelValue.getCN().getObjectName() + ",Model=" + model.getObjectName() + ",Vector=Reactions[" + reaction.getObjectName() + "],Reference=ParticleFlux>";
			modelValue.setExpression(expression);

			copasiReactionFluxValueByReaction.Add (reaction, modelValue);

			return modelValue;
		}

		public void CompileAndUpdate()
		{
			// finally compile the model
			// compile needs to be done before updating all initial values for
			// the model with the refresh sequence
			model.compileIfNecessary();
			
			// now that we are done building the model, we have to make sure all
			// initial values are updated according to their dependencies
			model.updateInitialValues(changedObjects);
			changedObjects.Dispose();

			changedObjects = new ObjectStdVector();
		}

		public void ExportSbml(string filename)
		{
			// export the model to an SBML file
			// we save to a file named example1.xml, we want to overwrite any
			// existing file with the same name and we want SBML L2V3
			try
			{
				dataModel.exportSBML("model.xml", true, 2, 3);
				// TODO: it is not possible to export the global quantities to SBML, the export fails. has to be fixed
			}
			catch
			{
				System.Console.Error.WriteLine("Error. Exporting the model to SBML failed.");
			}
		}

		public void SaveModel(string filename)
		{
			// save the model to a COPASI file
			// we save to a file named example1.cps 
			// and we want to overwrite any existing file with the same name
			// Default tasks are automatically generated and will always appear in cps
			// file unless they are explicitley deleted before saving.
			dataModel.saveModel(filename, true);
		}
	}
}

