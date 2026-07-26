import sys, os, subprocess, shutil, math

'''
Welcome to the Soulbound build script.

This builds Soulbound locally for every target, automatically.
The only requirements are Unity 2018.4.36f1, and 7-Zip if you wish to auto-package the builds too.
Just set up the options below in "Script Vars" to your liking and run the script!

Alternatively, you may run this script from the command line:

> Build.py [--single <target>] [--nozip]

Type `--single` followed by a number from 1 - 5 to build for a given target system listed on lines 54-60.
If not provided, the script will build for every possible build target defined in the script below.

You may also type `--nozip` either before or after `--single <target>`, or by itself, to prevent the script
from automatically zipping up your build(s) with 7-Zip.
If not provided, the script will automatically use 7-Zip to package all of your builds into .zip files.
'''

### Script Vars ###

# This names the build folders and the executables
version = "0.3.0"

# This is the path we will build to
buildPath = os.getcwd() + "\\bin"
if not "bin" in os.listdir():
    print("Creating \"bin\" folder...", end="")
    sys.stdout.flush()
    os.mkdir("bin")
    print("Done.")
else:
    print("\"bin\" folder already exists.")

# This is a list of paths to Unity. Add your own if it's not in the list.
unityPath = "C:\\Program Files\\Unity\\Hub\\Editor\\2018.4.36f1\\Editor\\Unity.exe"
if not os.path.exists(unityPath):
    unityPath = "C:\\Program Files\\Unities\\2018.4.36f1\\Editor\\Unity.exe"
if not os.path.exists(unityPath):
    unityPath = "C:\\Program Files\\Unity\\Editor\\Unity.exe"
if not os.path.exists(unityPath):
    sys.exit("None of the given paths to Unity's executable are valid. Please install Unity or edit your own path in this file.")

# This determines if this script will attempt to use 7-Zip to package the builds after they have been created
doPackage = True

# This is a list of paths to 7-Zip. Add your own if it's not in the list.
sevenZPath = "C:\\Program Files (x86)\\7-Zip\\7z.exe"
if not os.path.exists(sevenZPath):
    sevenZPath = "C:\\Program Files\\7-Zip\\7z.exe"
if not os.path.exists(sevenZPath):
    print("None of the given paths to 7-Zip's executable are valid. Please install 7-Zip or edit your own path in this file.")

### Actual code or whatever ###

# Define a list of pairs of folder names, command line arguments for Unity, and target names respectively
# * Mac is not in this list, see below
buildTargets = [
    ("Soulbound v" + version + " - Windows (32-bit)", "-buildWindowsPlayer",     "Soulbound.exe"),
    ("Soulbound v" + version + " - Windows (64-bit)", "-buildWindows64Player",   "Soulbound.exe"),
    ("Soulbound v" + version + " - Linux (32-bit)",   "-buildLinux32Player",     "Soulbound.x86"),
    ("Soulbound v" + version + " - Linux (64-bit)",   "-buildLinux64Player",     "Soulbound.x86_64")
]
macTarget = ("Soulbound v" + version + " - Mac",      "-buildOSXUniversalPlayer", "Soulbound.app")

# Files marked hidden in a build so players do not stumble over them. The joke
# button sprites that used to be listed here went with Crate Your Frisk in v0.3,
# and the overworld sprite folders went in v0.1.
hidePaths = []

def buildWithUnity(folder, argument, target):
    print("")
    if len(folder) < 76:
        print("╒" + ("═" * math.ceil((74 - len(folder)) / 2)) + folder + ("═" * math.floor((74 - len(folder)) / 2)) + "╕")
    else:
        print(folder)
    # Make sure destination folder doesn't exist
    if folder in os.listdir("bin"):
        print("Folder " + folder + " already exists, deleting...", end="")
        sys.stdout.flush()
        try:
            shutil.rmtree("bin\\" + folder)
            print("Done.")
        except:
            print("\n\nFatal error when attempting to delete \"bin\\" + folder + "\" folder. Exiting.\nYou should probably delete it manually.")
            sys.exit()

    # Build the Unity executable
    print("Begin Unity build for " + folder + "...", end="")
    sys.stdout.flush()
    subprocess.call([unityPath, "-batchmode", "-logFile " + buildPath + "\\output_" + folder + ".txt", argument, target, "-quit"])
    print("Done.")

    # Copy over the documentation
    print("Copying docs...", end="")
    sys.stdout.flush()
    shutil.copytree("docs", buildPath + "\\" + folder + "\\docs")
    print("Done.")

    # Copy over the Default and Mods folders
    print("Copying Default folder...", end="")
    sys.stdout.flush()
    os.system("xcopy \"" + buildPath + "\\Default\" \"" + buildPath + "\\" + folder + "\\Default\" /e /h /i > nul")
    print("Done.")

    print("Copying Mods folder...", end="")
    sys.stdout.flush()
    os.system("xcopy \"" + buildPath + "\\Mods\" \"" + buildPath + "\\" + folder + "\\Mods\" /e /h /i > nul")
    print("Done.")

    if len(folder) < 76:
        print("╘" + ("═" * 74) + "╛")
    print("")

# Make the ultimate Default folder which will be copied later
print("\nSetting up \"Default\".")
print("Copying Default folder...", end="")
sys.stdout.flush()
if "Default" in os.listdir("bin"):
    print("Folder \"Default\" already exists, deleting...", end="")
    sys.stdout.flush()
    try:
        shutil.rmtree(buildPath + "\\Default")
        print("Done.")
    except:
        print("\n\nFatal error when attempting to delete \"Default\" folder. Exiting.\nYou should probably delete it manually.")
        sys.exit()

shutil.copytree("Assets\\Default", buildPath + "\\Default")
print("Removing .meta files...", end="")
sys.stdout.flush()
for path,dirs,files in os.walk(buildPath + "\\Default"):
    for file in files:
        if file.endswith(".meta"):
            os.remove(path + "\\" + file)
print("Done.\n")

# Make the ultimate Mods folder which will be copied later
print("Setting up \"Mods\".")
print("Copying Mods folder...", end="")
sys.stdout.flush()
if "Mods" in os.listdir("bin"):
    print("Folder \"Mods\" already exists, deleting...", end="")
    sys.stdout.flush()
    try:
        shutil.rmtree("bin\\Mods")
        print("Done.")
    except:
        print("\n\nFatal error when attempting to delete \"Mods\" folder. Exiting.\nYou should probably delete it manually.")
        sys.exit()

shutil.copytree("Assets\\Mods", buildPath + "\\Mods")
print("Removing .meta files...", end="")
sys.stdout.flush()
for path,dirs,files in os.walk(buildPath + "\\Mods"):
    for file in files:
        if file.endswith(".meta"):
            os.remove(path + "\\" + file)
print("Done.")
print("Adding \"Mods starting with @ won't appear in the boss list\"...", end="")
sys.stdout.flush()
open(buildPath + "\\Mods\\Mods starting with @ won't appear in the boss list", "w").close()
print("Done.\n")

# Hide secret paths
print("Hiding secrets...", end="")
for path in hidePaths:
    os.system("attrib +h +s \"bin\\" + path + "\"")
print("Done.\n")

# Last step before building executables
print("Disabling allowFullscreenSwitch...", end="")
sys.stdout.flush()
psCurrent = open("ProjectSettings\\ProjectSettings.asset", "r")
settingsCurrent = psCurrent.read()
psCurrent.close()
ps = open("ProjectSettings\\ProjectSettings.asset", "w")
settings = settingsCurrent.replace("allowFullscreenSwitch: 1", "allowFullscreenSwitch: 0")
ps.write(settings)
ps.close()
print("Done.\n")

### Time to actually build the game ###

def buildForMac():
    # Now, the special behavior for Mac
    print("")
    if len(macTarget[0]) < 76:
        print("╒" + ("═" * math.floor((74 - len(macTarget[0])) / 2)) + macTarget[0] + ("═" * math.ceil((74 - len(macTarget[0])) / 2)) + "╕")
    else:
        print(macTarget[0])

    print("Enabling allowFullscreenSwitch...", end="")
    sys.stdout.flush()
    psCurrent = open("ProjectSettings\\ProjectSettings.asset", "r")
    settingsCurrent = psCurrent.read()
    psCurrent.close()
    ps = open("ProjectSettings\\ProjectSettings.asset", "w")
    settings = settingsCurrent.replace("allowFullscreenSwitch: 0", "allowFullscreenSwitch: 1")
    ps.write(settings)
    ps.close()
    print("Done.")

    # Make sure destination folder doesn't exist
    if macTarget[0] in os.listdir("bin"):
        print("Folder " + macTarget[0] + " already exists, deleting...", end="")
        sys.stdout.flush()
        try:
            shutil.rmtree("bin\\" + macTarget[0])
            print("Done.")
        except:
            print("\n\nFatal error when attempting to delete \"bin\\" + macTarget[0] + "\" folder. Exiting.\nYou should probably delete it manually.")
            sys.exit()

    # Build the Unity executable
    print("", end="")
    print("Begin Unity build for " + macTarget[0] + "...", end="")
    sys.stdout.flush()
    subprocess.call([unityPath, "-batchmode", "-logFile " + buildPath + "\\output_mac.txt", macTarget[1], buildPath + "\\" + macTarget[0] + "\\" + macTarget[2], "-quit"])
    print("Done.")

    # Copy over the documentation
    print("Copying docs...", end="")
    sys.stdout.flush()
    shutil.copytree("docs", buildPath + "\\" + macTarget[0] + "\\docs")
    print("Done.")

    # Copy over the Default and Mods folders
    print("Copying Default folder...", end="")
    sys.stdout.flush()
    os.system("xcopy \"" + buildPath + "\\Default\" \"" + buildPath + "\\" + macTarget[0] + "\\" + macTarget[2] + "\\Default\" /e /h /i > nul")
    print("Done.")

    print("Copying Mods folder...", end="")
    sys.stdout.flush()
    os.system("xcopy \"" + buildPath + "\\Mods\" \"" + buildPath + "\\" + macTarget[0] + "\\" + macTarget[2] + "\\Mods\" /e /h /i > nul")
    print("Done.")

    # Copy over the warning .txt file
    print("Copying \"How to run Soulbound on Mac.txt\"...", end="")
    sys.stdout.flush()
    shutil.copyfile("How to run Soulbound on Mac.txt", buildPath + "\\" + macTarget[0] + "\\How to run Soulbound on Mac.txt")
    print("Done.")

    print("Disabling allowFullscreenSwitch...", end="")
    sys.stdout.flush()
    psCurrent = open("ProjectSettings\\ProjectSettings.asset", "r")
    settingsCurrent = psCurrent.read()
    psCurrent.close()
    ps = open("ProjectSettings\\ProjectSettings.asset", "w")
    settings = settingsCurrent.replace("allowFullscreenSwitch: 1", "allowFullscreenSwitch: 0")
    ps.write(settings)
    ps.close()
    print("Done.")

    if len(macTarget[0]) < 76:
        print("╘" + ("═" * 74) + "╛")

### Command line parser ###

if len(sys.argv) > 1:
    try:
        # `--nozip` to make the program not auto-zip all outputted builds with 7-Zip (enabled by default)
        if sys.argv[1] == "--nozip":
            doPackage = sys.argv[1] != "--nozip"
        elif len(sys.argv) > 3 and sys.argv[3] == "--nozip":
            doPackage = sys.argv[3] != "--nozip"

        # provide `--single` as the first or second argument, followed by one number or "mac", to choose one specific build target (builds all if not provided)
        target = None
        if sys.argv[1] == "--single":
            target = sys.argv[2] == "5" and "mac" or buildTargets[int(sys.argv[2]) - 1]
        elif len(sys.argv) > 2 and sys.argv[2] == "--single":
            target = sys.argv[2] == "5" and "mac" or buildTargets[int(sys.argv[3]) - 1]

        # build target
        if target == "mac":
            buildForMac()
        elif target != None:
            buildWithUnity(target[0], target[1], buildPath + "\\" + target[0] + "\\" + target[2])
        else:
            # Copy of below code...
            for target in buildTargets:
                buildWithUnity(target[0], target[1], buildPath + "\\" + target[0] + "\\" + target[2])
            buildForMac()
    except:
        print("Failed to parse launch options. Try again.")
else:
    # Let's do it!
    for target in buildTargets:
        buildWithUnity(target[0], target[1], buildPath + "\\" + target[0] + "\\" + target[2])
    buildForMac()

### Post-build actions ###

# Delete Default and Mods
print("\nDeleting \"Default\" and \"Mods\"...", end="")
sys.stdout.flush()
try:
    shutil.rmtree("bin\\Default")
    shutil.rmtree("bin\\Mods")
    print("Done.")
except:
    print("Failed to delete. You should do so manually before your next build.")

# Auto-package all builds
if doPackage:
    print("\nBegin packaging all builds into archives through 7-Zip.")
    if not os.path.exists(sevenZPath):
        print("Missing 7-Zip executable.")
    else:
        binContents = os.listdir("bin")
        for build in [x for x in binContents if os.path.isdir(os.path.join(os.path.abspath("bin"), x)) and x not in ["Default", "Mods"]]:
            print("Creating \"" + build + ".zip\"...")
            sys.stdout.flush()
            subprocess.call([sevenZPath, "a", "-mx9", buildPath + "\\" + build + ".zip", buildPath + "\\" + build + "\\*", "-bso0", "-bsp0"])
            print("Done.")

# Congratulations :)
print("\n\n\nAll done!")
print("Test every build before release, and clean up their Mods folders if applicable.")
print("Have fun mooving boolet.\n")
os.system("pause")
