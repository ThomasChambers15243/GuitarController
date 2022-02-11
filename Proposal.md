# Proposal For Guitar Midi Controller Rythem Game
<!--Write LaTex Math Equations Here https://latexeditor.lagrida.com/-->
<!--https://www.fluke.com/en-us/learn/blog/power-quality/electrical-noise-and-transients#:~:text=Electrical%20noise%20is%20the%20result,it%20gets%20on%20signal%20circuits. -->
### What is it?

The game controller will be a modified guitar for use in a rhythm game. You play it like a normal eletric guitar, however the sound is produced by an arduino attached to the bridge of the guitar. LED lights will stretch the neck; 6 strips with one per string. These lights will represent represent the game. The lights will light up individually in such a way to create the illusion of a note (a lit up led on the fret board) on a certain fret. The aim of th game is to play the notes on teh guitar at the correct place on the led. Its a game but can also be used as a skill trainer for guitarists.

### Design

The controller will need a maxiumim of two arduinos. A minimium of 12 pins are needed for the lights and guitar strings, then theres power and any other componentes added. To clean thigns up, a second one could be used with them communicating through a wifi shield. The design will be proposed using one though.
The arduino will sit at the bridge of the guitar. Each string will have a potential difference of 5v from the analog pins. At rest, the strings will not make contract with the neck of the controller. Along the neck, at each fret, will be a resistor. Each resistor will have the same voltage along the neck, but a different potential difference. THis is because the resisteors will form a potential divider, running in series down the neck for each string.
When the user makes conection with string, and presses down at a fret, the string will make contact at the resistor, acting as a Vout. This will complete the circuit, giving the arduino at the bridge a voltage input. Since this voltage comes from a potential divider, the input voltage will be different for every fret. A sketch running on the arduino will take this unique voltage reading and compare it to a pre-defined *voltage-note* table. Once the adjacent note is known, it will feed this correct Hz back through an audio output and play the note (with any desired effects)
The desired voltage for each fret can be calulate by a ratio of the connect resistor and the sum of resistors in the circuit.

![lagrida_latex_editor](https://media.github.falmouth.ac.uk/user/766/files/4f477ce0-30aa-4c94-b921-fbe29cc1c13f)

#### Problems
- Voltage may get very small at the higher/lower frets
  -Since the voltage is split at at each fret, the voltage out at the frets furthest from the Voltage in (either the low or high frets) will be very small, since the out at the arduino is only 5v. 
- Eletrical noise at the smaller voltages
  - Since the voltages will get very small, the chance that the signals will get disrupted and mistaken for other potentials will be high. This noise could come from the user playing the strings, signal grounding, temperature changes and natural fluctuations.
  - Solutions to these are:
    - Have a high input voltage and step it down before the arduino reads it.
    - Have less frets, start with 5 and see how many can be added before it gets unreadable, to keep the voltage higher.

### Stretch Goals
- **LED effects** - Everytime you play a note, LED effect along the neck are *played*. Such as a *pushing* effect of lights up the neck, with them slowing falling back down or rippling effects from every note the user plays.
- **Bending Pedal** - Due to the current running through the strings, the strings **have** to stay disconnected from each other during the entire game. This makes bending the strings, a key part of some guitar styles, next to impossible. A way around this problem would be to add a pedal to the controller, which takes the currently play note and increases its Hz based on teh value given by the pedal. The pedal would measure analogue input, ranging from 1 to 3 semitones. The neutral position would be of no effect, and removing your foot from the pedal would reset the pedal to the neutral position

### Parts Needed
A standard 6-string guitar has 21 frets. Each fret would its own resistor, meaning 126 resistors in total. 5 strings would only require 30. 
5 strips of LED lights
Minimum of one arduino
(Arduino wifi Card)


