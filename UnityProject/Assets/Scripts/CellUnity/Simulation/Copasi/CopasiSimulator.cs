using System.Collections;
using System.Collections.Generic;
using org.COPASI;
using System.Diagnostics;
using System;

using CellUnity.Reaction;

namespace CellUnity.Simulation.Copasi
{
	public class CopasiSimulator : ISimulator
	{
		public CopasiSimulator()
		{
			InitCopasi();
			
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
			
			// create a compartment with the name cell and an initial volume of 5.0
			// microliter
			compartment = model.createCompartment("cell"); // TODO: remove static value
		}
		
		private CCopasiDataModel dataModel;
		private CModel model;
		private CTrajectoryTask trajectoryTask;
		CCompartment compartment;
		
		private double currentTime;
		
		private List<CopasiReactionGroup> reactionList = new List<CopasiReactionGroup>();
		Dictionary<MoleculeSpecies, CMetab> copasiMetabBySpecies = new Dictionary<MoleculeSpecies, CMetab>();
		
		private UpdateQueue updateQueue;
		
		public void Init(UpdateQueue updateQueue)
		{
			this.updateQueue = updateQueue;
		
			// TODO: implement Sync
			MoleculeSpecies[] species = new MoleculeSpecies[]{}; // cue.Species;
			ReactionType[] reactions = new ReactionType[]{}; // cue.ReactionTypes;
		
			// we have to keep a set of all the initial values that are changed during
			// the model building process
			// They are needed after the model has been built to make sure all initial
			// values are set to the correct initial value
			ObjectStdVector changedObjects = new ObjectStdVector();
			
			compartment.setInitialValue(0.000001);
			CCopasiObject obj = compartment.getInitialValueReference();
			changedObjects.Add(obj);
			
			// create metabolites
			
			foreach (var s in species)
			{
				CMetab metab = AddSpecies(changedObjects, s.Name, compartment, (int)s.InitialQuantity, CMetab.REACTIONS); // TODO: read out the real global quantity (there are maybe manual created molecules in the environment)
				copasiMetabBySpecies.Add(s, metab);
			}
			
			// create a reactions
			
			reactionList = new List<CellUnity.Simulation.Copasi.CopasiReactionGroup>(reactions.Length);
			
			for (int i = 0; i < reactions.Length; i++)
			{
				ReactionType r = reactions[i];
				
				AddReaction(
					changedObjects,
					r
					);
			}
			
			// finally compile the model
			// compile needs to be done before updating all initial values for
			// the model with the refresh sequence
			model.compileIfNecessary();
			
			// now that we are done building the model, we have to make sure all
			// initial values are updated according to their dependencies
			model.updateInitialValues(changedObjects);
			changedObjects.Dispose();
			changedObjects = null;
			
			InitSimulation();
			
			// save the model to a COPASI file
			// we save to a file named example1.cps 
			// and we want to overwrite any existing file with the same name
			// Default tasks are automatically generated and will always appear in cps
			// file unless they are explicitley deleted before saving.
			dataModel.saveModel("model.cps", true);
		}
		
		private CMetab[] MetabsBySpecies(MoleculeSpecies[] species)
		{
			CMetab[] result = new CMetab[species.Length];
			
			for (int i = 0; i < species.Length; i++)
			{
				result[i] = copasiMetabBySpecies[species[i]];
			}
			
			return result;
		}
		
		private CMetab AddSpecies(ObjectStdVector changedObjects, string name, CCompartment compartment, double quantity, int status)
		{
			CMetab metab = model.createMetabolite(name, compartment.getObjectName());
			metab.setInitialValue(quantity); // use setInitialValue for Number (not Concentration) see Doc 1.5.2.3 metabolites 
			metab.setStatus(status);
			CCopasiObject obj = metab.getInitialValueReference();
			changedObjects.Add(obj);
			
			return metab;
		}
		
		private CReaction AddReaction(ObjectStdVector changedObjects, ReactionType reactionType)
		{
			string name = reactionType.GetAutoName();
			
			// now we create a reaction
			CReaction reaction = model.createReaction(name);
			
			// we can set these on the chemical equation of the reaction
			CChemEq chemEq = reaction.getChemEq();
			
			CMetab[] substrates = MetabsBySpecies(reactionType.Reagents);
			foreach (CMetab item in substrates)
			{
				// add substrate with stoichiometry 1
				chemEq.addMetabolite(item.getKey(), 1.0, CChemEq.SUBSTRATE);
			}
			
			CMetab[] products = MetabsBySpecies(reactionType.Products);
			foreach (CMetab item in products)
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
				parameter.setDblValue(reactionType.Rate);
				CCopasiObject obj = parameter.getValueReference();
				changedObjects.Add(obj);
				
				foreach (var substrate in substrates)
				{
					reaction.addParameterMapping("substrate", substrate.getKey());   
				}
			}
			else
			{
				throw new System.Exception("Error. Could not find a kinetic law that conatins the term \"Constant\".");
			}
			
			
			
			// create global quantity value
			
			CModelValue modelValue = model.createModelValue(name);
			
			modelValue.setInitialValue(0);
			var obj2 = modelValue.getInitialValueReference();
			assert(obj2 != null);
			changedObjects.Add(obj2);
			
			// set the status to assignment
			modelValue.setStatus(CModelValue.ODE);
			// the assignment does not have to make sense
			string expression = "<CN=" + modelValue.getCN().getObjectName() + ",Model=" + model.getObjectName() + ",Vector=Reactions[" + reaction.getObjectName() + "],Reference=ParticleFlux>";
			modelValue.setExpression(expression);
			
			reactionList.Add(new CopasiReactionGroup(reaction, reactionType, modelValue));
			
			return reaction;
		}
		
		private void assert(bool p)
		{
			if (!p) { throw new Exception("assert"); }
		}
		
		private void InitSimulation()
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
			
			dataModel.getModel().setInitialTime(0.0);
			
			// set some parameters for the LSODA method through the method
			CTrajectoryMethod method = (CTrajectoryMethod)trajectoryTask.getMethod();
			
			CCopasiParameter parameter = method.getParameter("Absolute Tolerance");
			parameter.setDblValue(1.0e-12);
			
		}
		
		public SimulationStep Step(double stepDuration)
		{
			CTrajectoryProblem problem = (CTrajectoryProblem)trajectoryTask.getProblem();
			
			problem.setDuration(stepDuration);
			
			currentTime += stepDuration;
			
			try
			{
				// now we run the actual trajectory
				trajectoryTask.processWithOutputFlags(true, (int)CCopasiTask.NO_OUTPUT);
			}
			catch
			{
				if (CCopasiMessage.size() > 0)
				{
					throw new System.Exception("Running the time course simulation failed: " + CCopasiMessage.getAllMessageText(true));
				}
				
				throw new System.Exception("Running the time course simulation failed");
			}
			
			// Update the species properties that have changed
			ReactionCount[] reactionCount = new ReactionCount[reactionList.Count];
			
			for (int i = 0; i < reactionList.Count; i++)
			{
				CopasiReactionGroup r = reactionList[i];
				
				reactionCount[i] = r.CalcParticleFlux();
			}
			
			//UpdateSimulation();
			
			// clean up
			trajectoryTask.restore();
			
			return new SimulationStep(reactionCount);
		}
		
		/*
		private void UpdateSimulation()
		{
			for (uint i = 0; i < model.getMetabolites().size(); i++)
			{
				CMetab m = (CMetab)model.getMetabolites().get(i);
				Console.WriteLine(m.getObjectName() + ":\t" + m.getValue().ToString() + "\t { initial:  " + m.getInitialValue() + " }");
			}
		}
		*/
		
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
		
		public void Dispose()
		{
			// TODO: CCopasiRootContainer.removeDatamodel();
			
			// TODO: ?? CCopasiRootContainer::destroy();
		}
		
	}
}