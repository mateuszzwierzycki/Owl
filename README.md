![Logo](https://github.com/mateuszzwierzycki/Owl/blob/master/OwlLogo.png)

# Owl
The core libraries of the Owl framework.

## Intro
These libraries are a basic interface for machine-learning-oriented data pre- and post-processing.
Including normalization, processing and serialization methods, some visualizations, expression evaluation functions etc.
Created as an open-source basis for other developers and myself.

## Solutions
There are 3 solutions in this repository:
1. Owl.Core (the core Owl library declaring all the data types)
2. Owl.GH.Common (the library to use when developing GH plugins which use the GH_OwlTensor etc.)
3. Owl.ParamSetup (project showing how to setup your GH plugin to use the Owl.Core and Owl.GH.Common)

## How do I use that ? 
Take a look at the Owl.Core/Snippets folder. 
While it's all VB, once compiled you can use it in any .NET language (IronPython, C#, F# etc.) 
For C#-minded people who want to read the code better, there are free online VB<>C# converters.

## Usage
1. Owl.Core: Clone and build. No dependencies other than the native .NET
2. Owl.GH.Common and Owl.ParamSetup: Clone, add refs to GH_IO and Grasshopper, build.
