CellUnity
=========

Molecular dynamics in cells can be hard to comprehend. To facilitate the understanding of such processes, visualization tools can be used. The final goal is to provide such a tool to visualize molecular dynamics and reactions in order that a student can comprehend and understand the processes taking place. This project focuses on the three-dimensional representation of the molecules and the visualization of the reactions. Additionally the visualization shall be connected with a simulator. The result of the quantitative simulation shall be used for the representation in order that the visualisation is quantitative correct and therefore close to reality. To allow this tool to run on different platforms, the Unity game engine is used.

Background
----------
Most simulators are simulating molecular dynamics correctly, but it is difficult if not impossible to follow a specific reaction chain, which is necessary to understand a specific process. On the other hand, most representative visualizations are not very realistic. The final goal is to create a tool that is near to reality and still comprehensible. This project, however, only covers the basic functionality of that tool and therefore forms an initial part of it. Further goals will be implemented in future projects.

Goals
-----
The goal is to create a Unity project which provides the following features:

It shall be possible to import molecules into the scene from PDB files (Format definition: http://deposit.rcsb.org/adit/docs/pdb_atom_format.html ) or directly from the RCSB Protein Data Bank ( http://www.rcsb.org/pdb/ ) via the PDB ID. The user can define an initial quantity for each molecule species which shall be created at the start of the visualization.

The user shall be capable of defining chemical reactions to the system. For this project, simple reactions like A + B -> C are sufficient.

From the imported molecule species and the defined reactions, the tool shall create an SBML ( http://sbml.org ). The created SBML file shall be used as input for the simulator.

A suitable simulator shall be searched. The simulator should be accessible with Unity and should be capable of reading the created SBML file.

The quantitative simulation result shall be visualized in the scene in real time. That means that the number and type of reactions calculated by the simulator shall be performed in the scene. It does not matter which molecules exactly react, it is only important that the molecule species, the reaction type and the number of reactions is correct to provide a somehow realistic visualization.

The aim of providing a good rendering performance of multiple complex molecules is not part of this project.
