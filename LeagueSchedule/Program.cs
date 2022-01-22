DateTime start = DateTime.Now;

int[,] matches = new int[15, 16];
string[] content = (File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Inputfile.txt"))).Split("\"");
int week = 0;
List<int> teams = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
List<int> AllPossibleCombinations = new();

foreach (var row in content.Where(t => t != "" && t != ",\r\n" && t != "\r\n" && !(t.Trim().StartsWith(",//w") || t.Trim().StartsWith("//w"))))
{
    string[] weekMatches = row.Replace(":", ",").Split(",");
    for (int i = 0; i < weekMatches.Length; i++)
    {
        matches[week, i] = int.Parse(weekMatches[i]);
    }
    week++;
}

bool IsInFile(int team1, int team2)
{
    for (int i = 0; i < 15; i++)
    {
        for (int j = 0; j < 16; j += 2)
        {
            if ((matches[i, j] == team1 && matches[i, j + 1] == team2) || (matches[i, j] == team2 && matches[i, j + 1] == team1))
                return false;
        }
    }
    return true;
}

for (int i = 0; i < teams.Count - 1; i++)
{
    for (int j = i + 1; j < teams.Count; j++)
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
    if (match == 8)
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

for (int i = 5; i < 15; i++)
{
    int[] currentWeek = new int[16];
    var test = GenerateRow(0, currentWeek);
    FilterCombinations(currentWeek);

    for (int j = 0; j < 16; j++)
    {
        matches[i, j] = currentWeek[j];
    }
}

for (int i = 5; i < 15; i++)
{
    Console.Write("\"");
    for (int j = 0; j < 16; j += 2)
    {
        Console.Write($"{matches[i, j]}:{matches[i, j + 1]}");
        if (j != 14)
            Console.Write(",");
    }
    Console.Write($"\", //w{i+1}");
    Console.WriteLine();
}

TimeSpan timeForCalc = DateTime.Now.Subtract(start);

Console.WriteLine("Koha per gjenerimin e orarit eshte: "+ timeForCalc.ToString("c"));
Console.Read();