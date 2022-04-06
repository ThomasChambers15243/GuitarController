
import json
#Beautiful Soup
import bs4
import requests

# Script for scapping and cleaning note freqs from 0 to 8th ocatave
# From here...https://pages.mtu.edu/~suits/notefreqs.html

###
#Recursively finds the first alpha char in the star
###
def FindFistAlphaChar(string, index):
    if string[index].isalpha:
        return index
    else:
        return FindFistAlphaChar(string, index+1)
def FindFreq(string, index):
    if string[index].isdigit():
        return index
    else:
        return FindFreq(string, index+1)

url = "https://pages.mtu.edu/~suits/notefreqs.html"
page = requests.get(url)

soup = bs4.BeautifulSoup(page.content, "html.parser")
results = soup.findAll("table")
notes = results[1].findAll("tr")

data = []

# Load into data
for el in notes:
    data.append(el.text)



noteDict = {}

# Cleans data and loads it into dictionary that is
# then saved as a JSON File

for el in data:
    # Elemeants of the dictionary
    noteName = ""
    noteOctave = 0
    noteFrequency = 0.0

    # Find start of string
    index = FindFistAlphaChar(el,0)

    # Set note Name
    noteName = el[index]

    # Set octave, update name to sharp if needed and set freq

    # If its not a sharp
    if el[index+1].isdigit():
        noteOctave = int(el[index+1])
        # Find freq
        index = FindFreq(el,index+2)
        tempFreq = ""
        findingFreq = True
        while findingFreq:
            if el[index].isdigit:
                tempFreq+=el[index]
                index+=1
            if el[index] == ".":
                tempFreq+="."
                tempFreq+=el[index+1]
                tempFreq+=el[index+2]
                findingFreq = False
        noteFrequency = float(tempFreq)
    # If it is a sharp
    else:
        noteName += el[index+1]
        noteName += el[index+2]
        noteOctave = el[index+3]
        # Find freq
        index = FindFreq(el, index + 8)
        tempFreq = ""
        findingFreq = True
        while findingFreq:
            if el[index].isdigit:
                tempFreq += el[index]
                index += 1
            if el[index] == ".":
                tempFreq += "."
                tempFreq += el[index + 1]
                tempFreq += el[index + 2]
                findingFreq = False
        noteFrequency = float(tempFreq)

    # Add to the dictionary
    noteDict.update({noteName+str(noteOctave):{"Name":noteName+str(noteOctave),"Octave":str(noteOctave),"Freq":str(noteFrequency)}})

print("_______________-")
for key in noteDict:
    print(key + " : " + str(noteDict[key]))
print("_______________-")

# Load dictionary into json
# Then writes it to a json file
# Then reads the file for testing

#json_object_of_notes = json.dumps(noteDict, sort_keys=False, indent=4, separators=(',', ': '))

#with open("noteFreqs.json", "w") as outfile:
#    json.dump(json_object_of_notes, outfile)

#with open("noteFreqs.json") as infile:
#    newData = json.load(infile)
