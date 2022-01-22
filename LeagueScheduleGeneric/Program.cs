void Usage() =>
    Console.WriteLine("Perdorimi: LeagueScheduleGeneric.exe -k -f\n-k Numri i javëve\n-f Shtegu ku është fajlli hyrës\nShembull: LeagueScheduleGeneric.exe -k 15 -f D:\\Inputfile.txt");

if(args.Length < 4)
{
    Usage();
    return;
}

DateTime start = DateTime.Now;

int teamsNr;
int weekNr = int.Parse(args[1]);
string filePath = args[3];
int[] teams;
int[,] matches;
int weeksInFile = 0;

if (!File.Exists(filePath))
{
    Console.WriteLine("Shkruaj saktë shtegun e fajllit hyrës!");
    Usage();
    return;
}

string[] content = (File.ReadAllText(filePath)).Split("\"");
teamsNr = content.Where(t => t is not "" and not ",\r\n" and not "\r\n").FirstOrDefault().Replace(":", ",").Split(",").Length;
if(weekNr<=0 || weekNr > teamsNr - 1)
{
    Console.WriteLine("Nuk mund të gjenerohet orari me parametrat e kërkuar!");
    Console.WriteLine($"Nëse keni {teamsNr} ekipe maksimumi i numrit të javëve që mund të gjenerohet orari është {teamsNr-1} javë dhe minimumi i javëve të zgjedhura është 1 javë.");
    Usage();
    return;
}

matches = new int[weekNr, teamsNr];
teams = Enumerable.Range(1, teamsNr).ToArray();

foreach (var row in content.Where(t => t != "" && t != ",\r\n" && t != "\r\n" && !(t.Trim().StartsWith(",//w") || t.Trim().StartsWith("//w"))))
{
    string[] weekMatches = row.Replace(":", ",").Split(",");
    for (int i = 0; i < weekMatches.Length; i++)
    {
        matches[weeksInFile, i] = int.Parse(weekMatches[i]);
    }
    weeksInFile++;
    if (weekNr <= weeksInFile)
    {
        Console.WriteLine("Do të mbishkruhen javët në file!\nA dëshironi të vazhdoni: 1-Po, 0-Jo");
        int proceed = int.Parse(Console.ReadLine().ToString());
        if(proceed==1)
            break;
        else
            return;
    }
}

bool IsInFile(int team1, int team2)
{
    for (int i = 0; i < weekNr; i++)
    {
        for (int j = 0; j < teamsNr; j += 2)
        {
            if ((matches[i, j] == team1 && matches[i, j + 1] == team2) || (matches[i, j] == team2 && matches[i, j + 1] == team1))
                return false;
        }
    }
    return true;
}

List<int> AllPossibleCombinations = new();

for (int i = 0; i < teams.Length-1; i++)
{
    for (int j = i + 1; j < teams.Length; j++)
    {
        if (IsInFile(teams[i], teams[j]))
        {
            AllPossibleCombinations.Add(teams[i]);
            AllPossibleCombinations.Add(teams[j]);
        }
    }
}

bool GenerateRow(int match, int[] week)
{
    if (match == teamsNr/2)
        return true;

    if (match == 0)
    {
        week[0] = AllPossibleCombinations[0];
        week[1] = AllPossibleCombinations[1];
        match++;
        return GenerateRow(match, week);
    }

    for (int i = 0; i < AllPossibleCombinations.Count; i += 2)
    {
        if (IsValid(AllPossibleCombinations[i], AllPossibleCombinations[i + 1], week))
        {
            week[match * 2] = AllPossibleCombinations[i];
            week[match * 2 + 1] = AllPossibleCombinations[i + 1];
            match++;
            if (GenerateRow(match, week))
                return true;
            week[(match - 1) * 2] = 0;
            week[(match - 1) * 2 + 1] = 0;
            match--;
        }
    }
    return false;
}

bool IsValid(int teamA, int teamB, int[] week)
{
    for (int i = 0; i < week.Length; i++)
    {
        if ((week[i] == teamA || week[i] == teamB))
            return false;
    }

    return true;
}

void FilterCombinations(int[] array)
{
    List<int> indexesToDelete = new();
    for (int i = 0; i < AllPossibleCombinations.Count; i += 2)
    {
        for (int j = 0; j < array.Length; j += 2)
        {
            if (AllPossibleCombinations[i] == array[j] && AllPossibleCombinations[i + 1] == array[j + 1])
            {
                indexesToDelete.Add(i);
                indexesToDelete.Add(i + 1);
            }
        }
    }

    for (int i = 0; i < indexesToDelete.Count; i++)
        AllPossibleCombinations.RemoveAt(indexesToDelete[i] - i);
}

for (int i = weeksInFile; i < weekNr; i++)
{
    int[] currentWeek = new int[teamsNr];
    var test = GenerateRow(0, currentWeek);
    FilterCombinations(currentWeek);

    for (int j = 0; j < teamsNr; j++)
    {
        matches[i, j] = currentWeek[j];
    }
}

for (int i = weeksInFile; i < weekNr; i++)
{
    Console.Write("\"");
    for (int j = 0; j < teamsNr; j += 2)
    {
        Console.Write($"{matches[i, j]}:{matches[i, j + 1]}");
        if (j != teamsNr - 2)
            Console.Write(",");
    }
    Console.Write($"\", //w{i + 1}");
    Console.WriteLine();
}

TimeSpan timeForCalc = DateTime.Now.Subtract(start);
double memory = (float)GC.GetTotalMemory(true) / (1024);

Console.WriteLine("Koha per gjenerimin e orarit eshte: " + timeForCalc.ToString("c"));
Console.WriteLine("Hapsira ne memorie: "+memory.ToString("###,###.###################")+" KB");
Console.Read();