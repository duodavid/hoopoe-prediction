# Hoopoe Prediction

## Developer
Duong, David

## Introduction
Multi-agent system based programm for predicting hoopoe distribution in north germany

## Table of contents
1. [Simulation](#simulation)
2. [Software Stack](#software-Stack)
3. [How to setup?](#howtosetup)

## Run
The progam uses agents (hoopoes) to scout the environment. The environment is based on the data in the layers that are referencing the reallife environemnt.
The agents are given characteristics of typical hoopoes when choosing a place to stay. Based on these characteristics the agents select places in the environment where 
a hoopoe could be seen. The data are georeferenced, so you view the exact position using a geographical informations system. The program uses data with WGS 84 EPSG:4326 as the coordinate reference system.

## Software stack
This project is build on the C# multi-agent simulation framework MARS (Multi-Agent Research and Simulation) which provides all foundational structures to build a simulation environment with its agents and entities. The code for the simulation itself is written in C# whereas the visualization is written in python.
To run the program your system needs the following requirements.

- [Mars Live-Simulation 4.1.3 beta](https://www.nuget.org/packages/Mars.Life.Simulations/4.3.1-beta#readme-body-tab)
- [.Net 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

## How to setup?

### Simulation
1.  Download the repository from the Gitlab page

	If you have installed Git on your operating system, you can execute the following command in your console to clone the program:

	```bash
	 git clone .....
	 ```

	If Git is not installed on your system, you can download the game directly from our [repository](....) by clicking on the download icon in the right upper corner of the main page and download the game as a .zip file.

2. Open the programm
	After the download is finished, start your prefered C# IDE and open the project. We recommend you to use [Rider](https://www.jetbrains.com/de-de/rider/), but any other C# IDE will do it too.

	```
	└── 📁HoopoePrediction
	    ├── 📁HoopoePrediction
		      ├── 📄HoopoePrediction.csproj
	          └── 📄...
	```

3. In the situation that you dont know where to set the spawnpoints of the agents, set then variable runSim in Programm.cs to false and configure the parameters of the CreateSpawnpoints method in the "else"-block. 
   (Make sure the path to the spawnpoints.csv file is correct)

4. Before you start the simulation make sure to check the configuration (config.json) of the program. The files for the corresponding layers have to be in directly "Ressource" folder (no nesting) and the numbers at the  end of each filename have to be the same or the resulting data will be incorrect. The result files can be nested as long as the path in the congifuration file is correct. The result files are a copy of the "meadow" files, but named differently to differentiate the two.

5. You should be able to run the programm.



## Visualize
The programm is created in close dependency using QGIS (https://www.qgis.org/de/site/). So to visualize everything you have to download the programm, create a new project and drag the "OpenStreetMap" layer on the left side
to your workbench, to display the world map. To visualize the data that are used in the program, drag the files to qgis. 

(If the files are displayed incorrectly, make sure the project crs (german:kbs) is in the format WGS 84 EPSG:4326)
