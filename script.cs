// R e a d m e
// -----------
// 
// In this file you can include any instructions or other comments you want to have injected onto the 
// top of your final script. You can safely delete this file if you do not want any such comments.
// 
        
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set Runtime.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.
        }

        public void Save()
        {
            
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }


public void Main(string argument, UpdateType updateSource)
{
    var batteries = new List<IMyBatteryBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries); // Correctly populates the batteries list

    if (batteries.Count == 0)
    {
        Echo("No batteries found!"); // Debugging line
        return;
    }

    double totalStored = 0;
    double totalCapacity = 0;

    foreach (var battery in batteries)
    {
        Echo("Checking battery: " + battery.CustomName); // Debugging line
        Echo("Detailed Info: " + battery.DetailedInfo); // Debugging line to view the full string

        string[] lines = battery.DetailedInfo.Split('\n');
        double currentStored = 0;
        double maxStored = 0;

        foreach (var line in lines)
        {
            // Looking for the "Max Stored Power" and "Current Stored Power"
            if (line.StartsWith("Max Stored Power:", StringComparison.OrdinalIgnoreCase))
            {
                string[] parts = line.Substring("Max Stored Power:".Length).Trim().Split();
                if (parts.Length > 0)
                {
                    maxStored = ParsePower(parts[0]);
                }
            }
            else if (line.StartsWith("Current Stored Power:", StringComparison.OrdinalIgnoreCase))
            {
                string[] parts = line.Substring("Current Stored Power:".Length).Trim().Split();
                if (parts.Length > 0)
                {
                    currentStored = ParsePower(parts[0]);
                }
            }
        }

        totalStored += currentStored;
        totalCapacity += maxStored;
    }

    double percent = totalCapacity > 0 ? (totalStored / totalCapacity * 100.0) : 0;

    string output = "Battery Status\n";
    output += "----------------------\n";
    output += "Stored: " + totalStored.ToString("F2") + " MWh\n";
    output += "Max:    " + totalCapacity.ToString("F2") + " MWh\n";
    output += "Charge: " + percent.ToString("F1") + "%";

    Echo(output); // Final output to the programmable block terminal

    // Now find LCDs manually and check their names
    var lcds = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds);

    for (int i = 0; i < lcds.Count; i++)
    {
        var panel = lcds[i];
        if (panel.CustomName.Contains("Battery Status"))
        {
            panel.ContentType = ContentType.TEXT_AND_IMAGE;
            panel.WriteText(output);
        }
    }
}

// Helper function to parse "2.50 MWh" or "500 kWh" into a double (MWh)
double ParsePower(string input)
{
    input = input.Trim().ToUpper();
    double multiplier = 1.0;

    if (input.EndsWith("MWH"))
    {
        input = input.Replace("MWH", "").Trim();
    }
    else if (input.EndsWith("KWH"))
    {
        input = input.Replace("KWH", "").Trim();
        multiplier = 0.001;
    }

    double value;
    if (double.TryParse(input.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value))
    {
        return value * multiplier;
    }

    return 0;
}
