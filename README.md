# RPH / RhinoPlotHelper
This is a plugin for Rhinoceros 6 (SR18+) and 7 that helps to create document layouts.
It will support batch print-to-file, layout editing, layout objects preset, and more in the future.

For instructions on the installation of the plugin, see https://github.com/Chengxuan-Li/RPH/releases/tag/v0.1.0

## Rhino Commands Available at the Moment

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

``<Name>`` option the following options: ``<ISO Paper>``, ``Orientation``, ``<Custom Paper>``, allowing for the speficification of paper sizes and orientations respectively.

#### Scale

``<Name>`` option allows the user to specify the drawing scale (detail view scale) of the layout.

#### Name

``<Name>`` option allows the user to enter a custom name for the layout to be created. If it is not specified, the layout name will be marked with the first four characters of its GUID (Globally Unique Identifier)


