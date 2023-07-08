using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestUtils
{
  private static readonly System.Random random = new System.Random();

  private static readonly float[] ageWeights = GenerateAgeWeights();

  private static readonly string[] playerNames = new string[] {
    "Thomas",
    "Lilian",
    "Lilian",
    "Evan",
    "Evan",
    "Sacha",
    "Anatol",
    "Théotim",
    "Eden",
    "Marion",
    "Achraf",
    "Emilien",
    "Paulo",
    "Antoine",
    "Ilan",
    "Joris",
    "Lola",
    "Alice",
    "Anna",
    "Loick",
    "Theo",
    "Victor",

    "Lucas",
    "Ness",

    "Alekseï",
    "Henk",

    "Steve",
    "Maxine",
    "Chloe",
    "Rachel",
    
    "Greg",
    "James",
    "Lisa",
    "Rob",
    "Allison",
    "Eric",

    "Michael",
    "Jim",
    "Pam",
    "Dwight",
    "Andy",
    "Angela",
    "Kevin",
    "Oscar",
    "Ryan",
    "Kelly",
    "Meredith",
    "Creed",
    "Darryl",
    "Toby",
    "Stanley",
    "Phyllis",
    "Roy",
    "Jan",
    "David",
    "Karen",
    "Holly",
    "Erin",
    
    "Michael",
    "Eleanor",
    "Chidi",
    "Tahani",
    "Jason",
    "Janet",
    "Doug",

    "Redo",
    "Blu",
    "Gren",
    "Yello"
  };
  
  public static string GetRandomPlayerName() {
    return playerNames[Random.Range(0, playerNames.Length)];
  }


  private static float[] GenerateAgeWeights() 
  {
    float[] weights = new float[93];
    for (int i = 0; i < 93; i++)
    {
        float x = i + 8;
        float weight = Mathf.Exp(-Mathf.Pow(x - 22, 2) / (2 * Mathf.Pow(10, 2)));
        weights[i] = weight;
    }

    return weights;
  }

  public static int GetRandomPlayerAge()
  {
      float totalWeight = 0;
      foreach (float weight in ageWeights)
      {
          totalWeight += weight;
      }

      double randomNumber = random.NextDouble() * totalWeight;
      double weightSum = 0;
      for (int i = 0; i < 93; i++)
      {
          weightSum += ageWeights[i];
          if (randomNumber < weightSum)
          {
              return i + 8;
          }
      }

      return 99;
  }
}
