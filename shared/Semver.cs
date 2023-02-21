using System.Text.RegularExpressions;

namespace shared;

/// <summary>
/// A class for handling semver with sorting
/// </summary>
public struct Semver : IComparable<Semver>
{
   public readonly ushort Major { get; }
   public readonly ushort Minor { get; }
   public readonly ushort Patch { get; }
   
   public readonly string Text { get; }

   public readonly bool IsValid { get; }

   public Semver(string text)
   {
      Text = text;
      
      var regex = new Regex("^v([0-9]+).([0-9]+).([0-9]+)$");

      var match = regex.Match(text);
      if (!match.Success)
      {
         Major = 0;
         Minor = 0;
         Patch = 0;
         IsValid = false;
         return;
      }

      Major = Convert.ToUInt16(match.Groups[1].Value);
      Minor = Convert.ToUInt16(match.Groups[2].Value);
      Patch = Convert.ToUInt16(match.Groups[3].Value);

      IsValid = true;
   }

   public string GetConstraint()
   {
      return $"v{Major}.";
   }

   public int CompareTo(Semver other)
   {
      if (!IsValid || !other.IsValid)
      {
         // at least one of these is invalid
         if (IsValid == other.IsValid)
         {
            return 0;
         }
         
         if(IsValid)
         {
            return 1;
         }

         return -1;
      }

      var temp = Major.CompareTo(other.Major);
      if (temp != 0)
      {
         return temp;
      }
      
      temp = Minor.CompareTo(other.Minor);
      if (temp != 0)
      {
         return temp;
      }
      
      return Patch.CompareTo(other.Patch);
   }

   public override string ToString()
   {
      return Text;
   }
}