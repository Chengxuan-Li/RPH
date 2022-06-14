# RPH / RhinoPlotHelper
This is a plugin for Rhinoceros 6 that helps to create document layouts.
It will support batch print-to-file, layout editing, layout objects preset, and more.

## Rhino Commands (Future Deployments to be Included)

### RPHAddNewLayout

``RPHAddNewLayout`` is a command that adds a new layout according to a certain sets of options.

RPHAddNewLayout command exposes the following options:

#### Position

``<Position>`` option consists of the following modes of operation: ``<From_Point>``, ``<From_Content_Objects>``.

+ From_Point

``<From_Point>`` has one optional parameter: ``<Justification>`` which could be set to the desired justification method to guide the layout creation process.

+ From_Content_Objects

This option will allow the user to select one or a set of Rhino objects and will calculate the bounding box (world XY) of the selected object(s) automatically to be the layout extent.
After selection, the paper size will change automatically to fit the selected object(s) while remaining the drawing scale. The justification method will be switched to center.

#### Paper

#### Scale

#### Name

#### Edge

#### User_Attributes

