# Owl.ParamSetup

## Intro

This solution shows how to setup a VS project, so that the built plugin can use the same Tensor and TensorSet data as the Owl plugin.

## How

1. Get Owl.Core.dll and Owl.GH.Common.dll (by building them from the other projects provided in this repository)
2. Open the solution in VS and set up the reference paths. 
Be sure to set the CopyLocal property of Grasshopper.dll, GH_IO.dll, RhinoCommon.dll, Owl.GH.Common.dll, Owl.Core.dll to False. 
By setting it to True you will destroy the dependency trees of all the other plugins using those libraries.
3. Build. The library is compiled as a .gha file which you can install as a Grasshopper plugin. 
Be sure to have the Owl.Core.dll and Owl.GH.Common.dll in the same folder as the .gha file.
