# Owl.GH.Common

## Types (Goos)
There are 2 main types of data in the Owl plugin:
1. Tensors
2. TensorSets

Both of them need to be wrapped in a GH_Goo based class, which are called GH_OwlTensor and GH_OwlTensorSet.
This way Grasshopper knows what to do with this data and how to "parse" it. 
There are also some casting methods in those classes which makes sure it's easy to use them being end-user.

Additionally there is a GH_Trigger which can be used in time-based workflows.

## Parameters

Every GH_Goo needs a parameter which can read/write it.
This is the same parameter to which you connect the wires in Grasshopper. 
Parameters know what and how to do with Goos.
By using parameters provided here, you make sure your plugin/component can exchange the data with the Owl.GH plugin.

## Components
This section is not really well documented and treat those abstract component classes as alpha release. 
Probably the most useful is the ImageComponentBase which is a scalable component displaying and saving whatever it displays.
