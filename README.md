# Unreal Engine 4 save game converter
This library and simple console tool for it will convert the generic UE4 save game file into a json for easier analysis.

Bakc convertion is theoretically possible, but is not implemented.

Due to limitations of how UE4 serializes the data, some data types might be missing, and might fail deserialization for some games.
For example, I know for a fact that there's at least a Set collection type, and a lot of less-frequently used primitive types (non-4 byte ints, double, etc).

## How to: Test in Visual Studio
- clone the repo
- using the solution editor, set "GvasConverter" as Startup Project
- under Debug->GvasConverter Debug Properties put the path to your UGCLiveries_0.sav file, .tsw2liv files won't work for now
- Debug, the json file will be saved next to the UGCLiveries_0.sav file
