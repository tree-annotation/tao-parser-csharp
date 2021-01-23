#load "TaoParser.cs"

var p = new TaoParser();

Console.WriteLine(p.parse("key [value] array [[item1][item2][item3]]")); // should print the same string