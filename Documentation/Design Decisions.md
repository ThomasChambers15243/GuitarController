# Design Decisions 
Throughout development there were critical decision to solve problems which effect the end solution greatly, but were decided after testing and reasoning. 

### Electric string on an Acoustic
At the start of the project, it was decided to use an acoustic with electric strings, rather than a more standard combination.  Initial testing showed that electric strings produced a very consistent voltage  with negligible effect on resistance. A second hand acoustic was also chosen over any electric body for 

 1. Its a re-used material, thus keeping in line with the contracts requirements for sustainable builds.
 2. Removed all chance of extra conductivity from the metal parts of an electric. 

### Only 6 Frets
The controller has many more frets than the six used. Six was decided since it provides sizeable gaps in-between voltages, allowing large min-max range windows for each voltage reading without them overlapping. All possible notes (albeit at lower octaves) are playable in the 36 provided notes. 
### Copper Tape Connection 
The issue of connection between the wire and the potential divider circuit was the main problem, and there is no perfect solution. Copper tape showed its self as the more workable solution. It provides a close-to-real guitar playing experience to the player, by feeling similar to a normal guitar and is a constant, easily applicable conductive surface. Other options were to used raised bumps, conductive gloves, wires on the strings or a direct resistor connection on the neck. All of these would have removed from the player experience of playing the guitar, and ruined immersion.
