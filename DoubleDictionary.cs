using System;
using System.Collections.Generic;

namespace DoubleDict
{

    public interface IDubDictManySecondaries<in TPrimary, TSecondary>
    {
        List<TSecondary> GetSecondaryList(TPrimary key);
    }

    public interface IDubDictManyPrimaries<TPrimary, in TSecondary>
    {
        List<TPrimary> GetPrimaryList(TSecondary key);
    }

    /// <summary>
    /// A Dictionary where both sides are accessable as keys. 
    /// </summary>
    /// <typeparam name="TPrimary"></typeparam>
    /// <typeparam name="TSecondary"></typeparam>
    public abstract class DoubleDict<TPrimary, TSecondary> 
    {
        protected Dictionary<TPrimary, List<TSecondary>> PrimaryToSecondary;
        protected Dictionary<TSecondary, List<TPrimary>> SecondaryToPrimary;

        protected DoubleDict()
        {
            PrimaryToSecondary = new Dictionary<TPrimary, List<TSecondary>>();
            SecondaryToPrimary = new Dictionary<TSecondary, List<TPrimary>>();
        }

        /// <summary>
        /// Adds a pair to the double dictionary. 
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        public abstract void AddPair(TPrimary primary, TSecondary secondary);

        /// <summary>
        /// Removes a pair from the double dictionary.
        /// Throws an Exception if the element cannot be found. 
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        public void RemovePair(TPrimary primary, TSecondary secondary)
        {
            if (!ContainsPair(primary, secondary))
            {
                throw new System.Exception("Double Dict does not contain that pair");
            }
            PrimaryToSecondary[primary].Remove(secondary);
            if (PrimaryToSecondary[primary].Count == 0)
            {
                PrimaryToSecondary.Remove(primary);
            }
            SecondaryToPrimary[secondary].Remove(primary);
            if (SecondaryToPrimary[secondary].Count == 0)
            {
                SecondaryToPrimary.Remove(secondary);
            }
        }

        /// <summary>
        /// Removes the primary key from the double dictionary. 
        /// Doing so will remove all secondary keys associated with that primary key. 
        /// </summary>
        /// <param name="primary"></param>
        public void RemovePrimary(TPrimary primary)
        {
            var tempSecondaries = PrimaryToSecondary[primary];
            foreach (var secondary in tempSecondaries)
            {
                RemovePair(primary, secondary);
            }
        }


        /// <summary>
        /// Removes the secondary key from the double dictionary. 
        /// Doing so will remove all primary keys associated with that primary key. 
        /// </summary>
        /// <param name="secondary"></param>
        public void RemoveSecondary(TSecondary secondary)
        {
            var tempPrimaries =SecondaryToPrimary[secondary];
            foreach (var primary in tempPrimaries)
            {
                RemovePair(primary, secondary);
            }
        }

        /// <summary>
        /// Sets the primary and secondary keys. 
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        public virtual void SetPair(TPrimary primary, TSecondary secondary)
        {
            if (ContainsPair(primary, secondary))
            {
                RemovePair(primary, secondary);
            }
            AddPair(primary, secondary);
        }

        /// <summary>
        /// Checks is a pair of primary and seconday keys exist within the double dictionary. 
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        /// <returns> Returns true if the primary key exists. Returns false otherwise. </returns>
        public bool ContainsPair(TPrimary primary, TSecondary secondary)
        {
            bool pToS = PrimaryToSecondary[primary].Contains(secondary);
            bool sToP = SecondaryToPrimary[secondary].Contains(primary);

            if (pToS && sToP)
            {
                return true;
            }

            if (pToS || sToP) //XOR because of above statement
            {
                throw new Exception("DoubleDictionary out of sync");
            }

            return false;
        }

        /// <summary>
        /// Checks if the double dictionary contains the given primary key. 
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        /// <returns> Returns true if the primary key exists. Returns false otherwise. </returns>
        public bool ContainsPrimary(TPrimary primary)
        {
            return PrimaryToSecondary.ContainsKey(primary) && PrimaryToSecondary[primary] != null;
        }

        /// <summary>
        /// Checks if the double dictionary contains the given secondary key. 
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="secondary"></param>
        /// <returns> Returns true if the secondary key exists. Returns false otherwise. </returns>
        public bool ContainsSecondary(TSecondary secondary)
        {
            return SecondaryToPrimary.ContainsKey(secondary) && SecondaryToPrimary[secondary] != null;
        }

        /// <summary>
        /// Gets a list of the primary keys.
        /// </summary>
        /// <returns>A list of the primary keys</returns>
        public List<TPrimary> GetPrimaryKeys()
        {
            return new List<TPrimary>(PrimaryToSecondary.Keys);
        }

        /// <summary>
        /// Gets a list of the secondary keys.
        /// </summary>
        /// <returns>A list of the secondary keys</returns>
        public List<TSecondary> GetSecondaryKeys()
        {
            return new List<TSecondary>(SecondaryToPrimary.Keys);
        }
    }

    /// <summary>
    /// A one-to-one implementation of the double dictionary. 
    /// </summary>
    /// <typeparam name="TPrimary"></typeparam>
    /// <typeparam name="TSecondary"></typeparam>
    public class DoubleDictOneToOne<TPrimary, TSecondary> : DoubleDict<TPrimary, TSecondary> 
    {
        public override void AddPair(TPrimary primary, TSecondary secondary)
        {
            PrimaryToSecondary[primary] = new List<TSecondary> {secondary};
            SecondaryToPrimary[secondary] = new List<TPrimary> {primary};
        }

        public TPrimary GetPrimary(TSecondary key)
        {
            if (!SecondaryToPrimary.ContainsKey(key))
            {
                throw new Exception("Key not found");
            }
            return SecondaryToPrimary[key][0];
        }

        public TSecondary GetSecondary(TPrimary key)
        {
            if (!PrimaryToSecondary.ContainsKey(key))
            {
                throw new Exception("Key not found");
            }
            return PrimaryToSecondary[key][0];
        }
    }

    /// <summary>
    /// An implementation of the double dictionary class where each primary key stores a list of secondary keys, and vice versa. 
    /// </summary>
    /// <typeparam name="TPrimary"></typeparam>
    /// <typeparam name="TSecondary"></typeparam>
    public class DoubleDictOneToMany<TPrimary, TSecondary> : DoubleDict<TPrimary, TSecondary>, IDubDictManySecondaries<TPrimary, TSecondary>
    {
        public override void AddPair(TPrimary primary, TSecondary secondary)
        {
            if (!PrimaryToSecondary.ContainsKey(primary) || PrimaryToSecondary[primary] == null)
            {
                PrimaryToSecondary[primary] = new List<TSecondary>();
            }
            PrimaryToSecondary[primary].Add(secondary);
            SecondaryToPrimary[secondary] = new List<TPrimary> {primary};
        }

        public TPrimary GetPrimary(TSecondary key)
        {
            if (!SecondaryToPrimary.ContainsKey(key))
            {
                throw new Exception("Key not found");
            }
            return SecondaryToPrimary[key][0];
        }

        public List<TSecondary> GetSecondaryList(TPrimary key)
        {
            if (!PrimaryToSecondary.ContainsKey(key))
            {
                throw new Exception("Key not found");
            }
            return PrimaryToSecondary[key];
        }
    }

    public class DoubleDictManyToMany<TPrimary, TSecondary> : DoubleDict<TPrimary, TSecondary>, IDubDictManyPrimaries<TPrimary, TSecondary>, IDubDictManySecondaries<TPrimary, TSecondary>
    {
        public override void AddPair(TPrimary primary, TSecondary secondary)
        {
            bool pToS = PrimaryToSecondary[primary].Contains(secondary);
            bool sToP = SecondaryToPrimary[secondary].Contains(primary);

            if (pToS && sToP)
            {
                throw new Exception("Variables already synced");
            }

            if (pToS || sToP) //XOR because of above statement
            {
                throw new Exception("DoubleDictionary out of sync");
            }

            if (PrimaryToSecondary[primary] == null)
            {
                PrimaryToSecondary[primary] = new List<TSecondary>();
            }
            if (SecondaryToPrimary[secondary] == null)
            {
                SecondaryToPrimary[secondary] = new List<TPrimary>();
            }

            PrimaryToSecondary[primary].Add(secondary);
            SecondaryToPrimary[secondary].Add(primary);
        }

        public List<TPrimary> GetPrimaryList(TSecondary key)
        {
            if (!SecondaryToPrimary.ContainsKey(key))
            {
                throw new Exception("Key not found");
            }
            return SecondaryToPrimary[key];
        }

        public List<TSecondary> GetSecondaryList(TPrimary key)
        {
            if (!PrimaryToSecondary.ContainsKey(key))
            {
                throw new Exception("Key not found");
            }
            return PrimaryToSecondary[key];
        }
    }
}