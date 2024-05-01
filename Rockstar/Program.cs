using System.Threading.Tasks;

public sealed class Solution
{
    private static int Main()
    {
        Console.WriteLine(
            "Test"
        );
        Console.ReadLine();
        return 0;
    }

    /// <summary>
    /// Parses an incoming array of integers for the number of repetitions of the integer values, e.g., 
    /// the number of times 1 occurs in the array & ensures the repetitions are equivalent to the value, e.g.,
    /// 8 == 8
    /// </summary>
    /// <param name="A">Array of integer values from -int.Max to int.Max.</param>
    /// <returns>The greatest value that is equivalent to it's repetitions, e.g., 8 == 8.</returns>
    public int solution(
        int[] A
    )
    {
        // order our sorted mapping in descending order
        var comparer = Comparer<int>.Create(
            (x, y) =>
            {
                return y.CompareTo(
                    x
                );
            }
        );

        // map values for the amount of times occurring - will contain values equal or less than
        SortedDictionary<int, int> repetitionMappings = new(
            comparer    
        );

        // contains values that exceeded the threshold
        Dictionary<int, int> removedMappings = new();

        // map values from incoming data set
        foreach (int value in A)
        {
            // all values less than or equal to 0 cannot meet the requirement
            if (value <= 0)
            {
                continue;
            }

            // did this value cross the threshold of it's own value, e.g., 3 repeated 4 times.
            bool wasMappingRemoved = removedMappings.ContainsKey(
                value
            );
            if (wasMappingRemoved)
            {
                continue;
            }

            // was this value previously added to our mapping
            bool wasMappingAdded = repetitionMappings.ContainsKey(
                value
            );
            if (wasMappingAdded)
            {
                // increase the value
                repetitionMappings[value]++;

                // remove if we've cross it's own value, e.g., 3 repeated 4 times
                bool isRepetitionTooMuch = repetitionMappings[value] > value;
                if (isRepetitionTooMuch)
                {
                    // we've crossed the threshold, remove this value from the sorted list & add to removed mapping
                    repetitionMappings.Remove(
                        value
                    );
                    removedMappings.Add(
                        value,
                        0
                    );
                }
            }
            else
            {
                // add the initial repetition
                repetitionMappings.Add( 
                    value,
                    1
                );
            }
        }

        // determine highest value that is equal to it's repetitions
        foreach (var repetitionMapping in repetitionMappings)
        {
            int integer = repetitionMapping.Key;
            int repetitions = repetitionMapping.Value;
            if (integer == repetitions)
            {
                return integer;
            }
        }

        return 0;
    }

    // Number Range: 0 to 1,048,575 -- (2^20 - 1) == 1048575
    // all values are positive as incoming integer data is declared as unsigned integers

    public int solution(String S)
    {
        return Operate(
            S    
        );
    }

    private enum ErrorCode : int
    { 
        Fail = -1,
        Success = 0,
    }

    private const uint c_maximumValue = 1048575u;
    private const uint c_minimumValue = 0u;
    private const string c_removeLastValue = "POP";
    private const string c_duplicateLastValue = "DUP";
    private const string c_addLastTwoValues = "+";
    private const string c_subtractLastTwoValues = "-";
    private const char c_operationDelimiter = ' ';

    private Stack<uint> m_values = new();

    private bool IsGreaterThanMaximumValue(
        int value
    )
    {
        return value > c_maximumValue;
    }

    private bool IsLessThanMinimumValue(
        int value
    )
    {
        return value < c_minimumValue;
    }

    private bool IsDoubleValueOperationValid()
    {
        return m_values.Count > 1u;
    }

    private bool IsSingleValueOperationValid()
    {
        return m_values.Count > 0u;
    }

    private int Operate(
        string input    
    )
    {
        // remove any old values from previous functions
        m_values.Clear();

        // split our incoming string data into integers & operation components
        string[] split = input.Split(
            c_operationDelimiter
        );
        foreach (string s in split)
        {
            bool isValue = uint.TryParse(
                s,
                out uint value
            );
            if (isValue)
            {
                // values are guaranteed to be between minimum & maximum value, safe to add
                m_values.Push(
                    value
                );
            }
            else
            {
                switch (s)
                {
                    case c_removeLastValue:
                        OperateOnRemoval();
                        break;

                    case c_duplicateLastValue:
                        OperateOnDuplication();
                        break;

                    case c_addLastTwoValues:
                        OperateOnAddition();
                        break;

                    case c_subtractLastTwoValues:
                        OperateOnSubtraction();
                        break;

                    // this is not a valid operation
                    default:
                        return (int)ErrorCode.Fail;
                }
            }
        }

        return (int)m_values.Peek();
    }

    private ErrorCode OperateOnAddition()
    {
        // ensure this is a valid operation, this requires two values
        bool isOperationValid = IsDoubleValueOperationValid();
        if (!isOperationValid)
        {
            return ErrorCode.Fail;
        }

        // pop the last two values as integers for addition
        int valueLast = (int)m_values.Pop();
        int valueSecondFromLast = (int)m_values.Pop();

        // values greater than our maximum value result in an error
        int result = valueLast + valueSecondFromLast;
        bool isGreaterThanMaximum = IsGreaterThanMaximumValue(
            result
        );
        if (isGreaterThanMaximum)
        {
            return ErrorCode.Fail;
        }

        m_values.Push(
            (uint)result
        );

        return ErrorCode.Success;
    }

    private ErrorCode OperateOnDuplication()
    {
        // ensure this is a valid operation, this requires two values
        bool isOperationValid = IsSingleValueOperationValid();
        if (!isOperationValid)
        {
            return ErrorCode.Fail;
        }

        // retrieve last value & add another to the stack
        int valueLast = (int)m_values.Last();
        m_values.Push(
            (uint)valueLast
        );

        return ErrorCode.Success;
    }

    private ErrorCode OperateOnRemoval()
    {
        // ensure this is a valid operation, this requires two values
        bool isOperationValid = IsSingleValueOperationValid();
        if (!isOperationValid)
        {
            return ErrorCode.Fail;
        }

        // pop the last value
        _ = m_values.Pop();

        return ErrorCode.Success;
    }

    private ErrorCode OperateOnSubtraction()
    {
        // ensure this is a valid operation, this requires two values
        bool isOperationValid = IsDoubleValueOperationValid();
        if (!isOperationValid)
        {
            return ErrorCode.Fail;
        }

        // pop the two last values as integers for subtraction
        int valueLast = (int)m_values.Pop();
        int valueSecondFromLast = (int)m_values.Pop();

        // values less than minimum result in an error
        int result = valueLast - valueSecondFromLast;
        bool isLessThanMinimum = IsLessThanMinimumValue(
            result
        );
        if (isLessThanMinimum)
        {
            return ErrorCode.Fail;
        }

        m_values.Push(
            (uint)result
        );

        return ErrorCode.Success;
    }
}

public abstract class Object
{
    public uint UniqueIdentifier { get; set; } = 0u;
    public string Name { get; set; } = string.Empty;
}

public abstract class Shop : Object
{

}

public sealed class PizzaShop : Shop
{
    public Dictionary<PizzaType, Pizza> AvailablePizzas = new();
    public DateTime OpenTime { get; set; } = DateTime.MinValue;
    public DateTime CloseTime { get; set; } = DateTime.MaxValue;
}

public abstract class Item : Object
{

}

public abstract class Consumable : Item
{

}

public enum PizzaType : uint
{
    Pepperoni = 0u,
    Hawaiian = 1u,
    MeatLovers = 2u,
}

public enum PizzaSize : uint
{
    Small = 0u,
    Medium = 1u,
    Large = 2u,
    ExtraLarge = 3u,
}

public sealed class Pizza : Consumable
{
    public PizzaType Type { get; set; } = PizzaType.Pepperoni;
    public PizzaSize Size { get; set; } = PizzaSize.Small;
}