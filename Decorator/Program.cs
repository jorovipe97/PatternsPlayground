using System;
using System.Collections.Generic;
using System.Text;

//You could certainly argue that
//you have to manage more objects with
//the Decorator Pattern and so there is
//an increased chance that coding errors
//will introduce the kinds of problems you
//suggest.However, decorators are typically
//created by using other patterns like Factory
//and Builder.Once we’ve covered these
//patterns, you’ll see that the creation of the
//concrete component with its decorator is
//“well encapsulated” and doesn’t lead to
//these kinds of problems.
namespace Decorator
{
    class Program
    {
        static void Main(string[] args)
        {
            Beverage beverage1 = new HouseBlend();
            beverage1 = new Milk(beverage1);
            beverage1 = new Soy(beverage1);
            beverage1 = new Soy(beverage1);
            beverage1.Size = CupSize.Small;
            Console.WriteLine(string.Format("{0}: $ {1}", beverage1.GetDescription(), beverage1.Cost()));
            beverage1.Size = CupSize.Medium;
            Console.WriteLine(string.Format("{0}: $ {1}", beverage1.GetDescription(), beverage1.Cost()));
            beverage1.Size = CupSize.Large;
            Console.WriteLine(string.Format("{0}: $ {1}", beverage1.GetDescription(), beverage1.Cost()));
            beverage1 = new PrettyDescriptionDecorator(beverage1);
            Console.WriteLine(string.Format("{0}: $ {1}", beverage1.GetDescription(), beverage1.Cost()));
            Console.ReadKey();
        }

        public enum CupSize
        {
            Small,
            Medium,
            Large
        }
        // base component
        public abstract class Beverage
        {
            public abstract CupSize Size { get; set; }
            public abstract string GetDescription();
            public abstract float Cost();
        }

        // concrete component A
        public class HouseBlend : Beverage
        {
            CupSize size;
            public override CupSize Size { get => size; set => size = value; }

            public override string GetDescription()
            {
                return Size.ToString() + " House Blend";
            }

            public override float Cost()
            {
                return 3f;
            }
        }

        // concrete component b
        public class DarkRoast : Beverage
        {
            CupSize size;
            public override CupSize Size { get => size; set => size = value; }

            public override string GetDescription()
            {
                return Size.ToString() + " Dark Roast";
            }

            public override float Cost()
            {
                return 2f;
            }
        }

        // decorator base
        public abstract class CondimentDecorator : Beverage
        {
            public override CupSize Size { get => wrappedObject.Size; set => wrappedObject.Size = value; }
            public Beverage wrappedObject;
            public CondimentDecorator(Beverage beverage)
            {
                wrappedObject = beverage;
            }
        }

        // concrete decorator A
        public class Milk : CondimentDecorator
        {
            public Milk(Beverage beverage) : base(beverage) { }

            public override string GetDescription()
            {
                return wrappedObject.GetDescription() + ", Milk";
            }

            public override float Cost()
            {
                switch (Size)
                {
                    case CupSize.Small:
                        return wrappedObject.Cost() + 1f;
                    case CupSize.Large:
                        return wrappedObject.Cost() + 2f;
                    case CupSize.Medium:
                    default:
                        return wrappedObject.Cost() + 1.5f;
                }
            }
        }

        // concrete decorator B
        public class Soy : CondimentDecorator
        {
            public Soy(Beverage beverage) : base(beverage) { }

            public override string GetDescription()
            {
                return wrappedObject.GetDescription() + ", Soy";
            }

            public override float Cost()
            {
                switch (Size)
                {
                    case CupSize.Small:
                        return wrappedObject.Cost() + 0.5f;
                    case CupSize.Large:
                        return wrappedObject.Cost() + 1.5f;
                    case CupSize.Medium:
                    default:
                        return wrappedObject.Cost() + 1f;
                }
            }
        }

        public class PrettyDescriptionDecorator : CondimentDecorator
        {
            public PrettyDescriptionDecorator(Beverage beverage) : base(beverage) { }

            public override string GetDescription()
            {
                string original = wrappedObject.GetDescription();
                string[] items = original.Split(',');
                if (items.Length == 0)
                    return "Invalid product description";

                string productName = items[0];
                Dictionary<string, int> inspectedCondiments = new Dictionary<string, int>();
                for (int i = 1; i < items.Length; i++)
                {
                    string condiment = items[i].Trim();
                    // how many times is current condiment ahead?
                    if (inspectedCondiments.ContainsKey(condiment))
                    {
                        inspectedCondiments[condiment] += 1;
                    }
                    else
                    {
                        inspectedCondiments.Add(condiment, 1);
                    }
                }

                string pretty = productName;
                foreach (KeyValuePair<string, int> pair in inspectedCondiments)
                {
                    pretty += string.Format(", {0} {1}", pair.Value, pair.Key);
                }

                return pretty;
            }

            public override float Cost()
            {
                return wrappedObject.Cost();
            }
        }


    }
}
