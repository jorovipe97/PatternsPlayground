#define CONTRACTS_FULL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

/// <summary>
/// The design by contract is a tecnique as important in OOP world as classes
/// objects, polymorphysm, etc.
/// 
/// The design by contract provides a way to think about the inheritance by stating 
/// that a subclass can't simply override an operation of its base class arbitrary
/// instead it have to be a subcontract of the operation in base class.
/// 
/// "Operations in subclasses cannot arbitrarily redefine/override operations in superclasses"
/// 
/// In the overriden operation in the subclass:
/// 1. The precondition must not be stronger than the precondition in the superclass
/// 2. The postcondition must not be weaker than the postcondition in the superclass
/// 3. The class invariant must not be weaker than the invariant in the superclass
/// 
/// If we assume that the precondition of a redefined operation in a subclass is stronger
/// than the precondition of the original operation in the superclass, then the subclass
/// cannot be used as a subcontractor of the superclass. Consequently, the preconditions of
/// redefined operations in subclasses must be equal to or weaker than the preconditions of
/// corresponding operations in superclasses.
/// 
/// In case the postcondition of a redefined operation in a subclass is weaker than the
/// postcondition of the operation in the superclass, the redefined operation does not
/// solve the problem as promised by the contract in the superclass. Therefore, the
/// postconditions of redefined operations must be equal to or stronger than the postconditions
/// of corresponding operations in superclasses.
/// 
/// The superclass has promised to solve some problem via the virtual operations.
/// Redefined and overridden operations in subclasses are obliged to solve the problem
/// under the same, or possible weaker conditions. This causes the weakening of preconditions.
/// The job done by the redefined and overridden operations must be as least as good as
/// promised in the superclass. This causes the strengthening of postconditions.
/// 
/// The invariant of the superclass expresses requirements to instance variables in the
/// superclass, at stable points in time. These instance variables are also present in subclasses,
/// and the requirements to these persist in subclasses. Consequently, class invariants cannot
/// be be weakened in subclasses.
/// 
/// Understand this concepts are key to understand how to follow and apply the best possible the
/// Liskov's principle as described by uncle bob on its SOLID principles.
/// 
/// Here I'm going to use c# contracts APIs to implement the ellipse/circle dilema described on its paper.
/// 
/// References:
/// Solid principles: https://web.archive.org/web/20150906155800/http://www.objectmentor.com/resources/articles/Principles_and_Patterns.pdf
/// Design by contractcs: https://archive.eiffel.com/doc/manuals/technology/contract/
/// Subcontracting and inheritance: http://people.cs.aau.dk/~normark/oop-csharp/html/notes/contracts_themes-subcontract-sect.html#contracts_inheritance-and-contracts_image_img1
/// Contracts in c#: https://docs.microsoft.com/en-us/dotnet/framework/debug-trace-profile/code-contracts
/// Easy contracts: http://blog.stephencleary.com/2011/01/simple-and-easy-code-contracts.html
/// 
/// Sadly i wasn't able to make this work, since contracts seems not well suported on VS2019, and i'm too lazy to test another libraries the importan here
/// is the concept.
/// </summary>
namespace DesignByContracts
{
    class Program
    {
        static void Main(string[] args)
        {
            Point focusA = new Point { x = 1, y = 0 };
            Point focusB = new Point { x = -1, y = 0 };
            Ellipse e = new Ellipse();
            e.FocusA = focusA;
            e.FocusB = focusB;
            e.MajorAxis = 10;
            Contract.Assert(e.FocusA == focusA);
            Contract.Assert(e.FocusB == focusB);

            e = new Circle();
            e.FocusA = focusA;
            e.FocusB = focusB;
            e.MajorAxis = 10;
            Console.ReadKey();
        }
    }

    struct Point
    {
        public float x;
        public float y;
        public static bool operator ==(Point a, Point b)
        {
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator !=(Point a, Point b)
        {
            return a.x != b.x || a.y != b.y;
        }
    }

    [ContractClassFor(typeof(Ellipse))]
    class EllipseContract : Ellipse
    {
        public override Point FocusA
        {
            get
            {
                return default(Point);
            }
            set
            {
                focusA = value;
                // Postconditions
                // ensures the other focus wasn't modified on this setter.
                Contract.Ensures(((Ellipse)this).focusB == Contract.OldValue(((Ellipse)this).focusB));
            }
        }

        public override Point FocusB
        {
            get
            {
                return default(Point);
            }
            set
            {
                focusB = value;
                // Postconditions
                // ensures the other focus wasn't modified on this setter.
                Contract.Ensures(((Ellipse)this).focusA == Contract.OldValue(((Ellipse)this).focusA));
            }
        }
    }

    [ContractClass(typeof(EllipseContract))]
    class Ellipse
    {
        // All members mentioned in a contract must be at least as visible as the method in which they appear. For example, a private field cannot be mentioned in a precondition for a public method; clients cannot validate such a contract before they call the method. However, if the field is marked with the ContractPublicPropertyNameAttribute, it is exempt from these rules.
        // [ContractPublicPropertyName("publicFocusA")]
        public Point focusA;
        public virtual Point FocusA {
            get
            {
                return focusA;
            }
            set
            {
                focusA = value;
            }
        }
        public Point focusB;
        public virtual Point FocusB {
            get
            {
                return focusB;
            }
            set
            {
                focusB = value;
            }
        }
        public float majorAxis;
        public virtual float MajorAxis
        {
            get
            {
                return default(float);
            }
            set
            {
                majorAxis = value;
            }
        }
    }

    class Circle : Ellipse
    {
        public override Point FocusA
        {
            get
            {
                return this.focusA;
            }
            set
            {
                this.focusA = value;
                this.focusB = value;
            }
        }

        public override Point FocusB
        {
            get
            {
                return this.focusB;
            }
            set
            {
                // to allow this, we would need to weak post conditions (remove conditions)
                // and remember that post conditions on subclasses can only be as strength or stronger than the base class operation overriden.
                this.focusA = value;
                this.focusB = value;
            }
        }
    }
}
