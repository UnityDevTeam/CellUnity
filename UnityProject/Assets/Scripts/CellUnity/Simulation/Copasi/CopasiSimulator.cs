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
			// microliter as the volume units
			// and nanomole as the substance units
			model.setTimeUnit(CUnit.s);
			model.setVolumeUnit(CUnit.microl);
			model.setQuantityUnit(CUnit.nMol);
		}
		
		private CCopasiDataModel dataModel;
		private CModel model;
		private CTrajectoryTask trajectoryTask;
		
		private double currentTime;
		
		private Dictionary<CReaction, ReactionType> reactionTypeByCopasiReaction = new Dictionary<CReaction, ReactionType>();
		
		public void Init(MoleculeSpecies[] species, ReactionType[] reactions)
		{
			// we have to keep a set of all the initial values that are changed during
			// the model building process
			// They are needed after the model has been built to make sure all initial
			// values are set to the correct initial value
			ObjectStdVector changedObjects = new ObjectStdVector();
			
			// create a compartment with the name cell and an initial volume of 5.0
			// microliter
			CCompartment compartment = model.createCompartment("cell", 5.0); // TODO: remove static value
			CCopasiObject obj = compartment.getInitialValueReference();
			changedObjects.Add(obj);
			
			// create metabolites
			
			Dictionary<MoleculeSpecies, CMetab> copasiMetabBySpecies = new Dictionary<MoleculeSpecies, CMetab>();
			
			foreach (var s in species) {
				CMetab metab = AddSpecies(changedObjects, s.Name, compartment, 10.0, CMetab.REACTIONS); // TODO: set particle number instead of concentration
				copasiMetabBySpecies.Add(s, metab);
			}
			
			// create a reactions
			
			foreach (var r in reactions) {
				CReaction reaction = AddReaction(
					changedObjects, 
					r.GetAutoName(), 
					MetabsBySpecies(copasiMetabBySpecies, r.Reagents),
					MetabsBySpecies(copasiMetabBySpecies, r.Products), 
					r.Rate
				);	
				
				reactionTypeByCopasiReaction.Add(reaction, r);
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
			
			// export the model to an SBML file
			// we save to a file named example1.xml, we want to overwrite any
			// existing file with the same name and we want SBML L2V3
			try
			{
				dataModel.exportSBML("model.xml", true, 2, 3);
			}
			catch
			{
				System.Console.Error.WriteLine("Error. Exporting the model to SBML failed.");
			}
		}
		
		private CMetab[] MetabsBySpecies(Dictionary<MoleculeSpecies, CMetab> copasiMetabBySpecies, MoleculeSpecies[] species)
		{
			CMetab[] result = new CMetab[species.Length];
			
			for (int i = 0; i < species.Length; i++) {
				result[i] = copasiMetabBySpecies[species[i]];
			}
			
			return result;
		}
		
		private CMetab AddSpecies(ObjectStdVector changedObjects, string name, CCompartment compartment, double iconc, int status)
		{
			// TODO: add; use setInitialValue for Number (not Concentration) see Doc 1.5.2.3 metabolites 
			
			CMetab metab = model.createMetabolite(name, compartment.getObjectName(), iconc, status);
			CCopasiObject obj = metab.getInitialValueReference();
			changedObjects.Add(obj);
			
			return metab;
		}
		
		private CReaction AddReaction(ObjectStdVector changedObjects, string name, CMetab[] substrates, CMetab[] products, double rate)
		{
			// now we create a reaction
			CReaction reaction = model.createReaction(name);
			
			// we can set these on the chemical equation of the reaction
			CChemEq chemEq = reaction.getChemEq();
			
			foreach (CMetab item in substrates)
			{
				// add substrate with stoichiometry 1
				chemEq.addMetabolite(item.getKey(), 1.0, CChemEq.SUBSTRATE);
			}
			
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
				// we just assume that the only suitable function with Constant in
				// it's name is the one we want
				if (suitableFunctions[i].getObjectName().IndexOf("Constant") != -1)
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
			}
			else
			{
				throw new System.Exception("Error. Could not find a kinetic law that conatins the term \"Constant\".");
			}
			
			return reaction;
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
			UpdateSimulation();
			
			trajectoryTask.restore();
			
			return new SimulationStep(null);
		}
		
		private void UpdateSimulation()
		{
			for (uint i = 0; i < model.getMetabolites().size(); i++)
			{
				CMetab m = (CMetab)model.getMetabolites().get(i);
				Console.WriteLine(m.getObjectName() + ":\t" + m.getValue().ToString() + "\t { initial:  " + m.getInitialValue() + " }");
			}
			
			for (uint j = 0; j < model.getModelValues().size(); ++j)
			{
				CModelValue modelValue = (CModelValue)model.getModelValues().get(j);
				string modelValueName = modelValue.getObjectName();
				
				Console.WriteLine("mv "+modelValueName+":\t"+modelValue.getValue().ToString());
				
				//if(reactionName == modelValueName)
	            //{
	            //    reaction->update(modelValue->getValue());
	            //    reactionUpdated = true;
	            //    break;
	            //}
	            
			}
		}
		
		public void ExportSbml()
		{
			
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