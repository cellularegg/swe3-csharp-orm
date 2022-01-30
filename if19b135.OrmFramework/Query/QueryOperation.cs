namespace if19b135.OrmFramework.Query
{
    internal enum QueryOperation : int
    {
        NO_OPERATION = 0,
        NOT = 1,
        AND = 2,
        OR = 3,
        BEGIN_GROUP = 4,
        END_GROUP = 5,
        EQUALS = 6,
        LIKE = 7,
        IN = 8,
        GREATER_THAN = 9,
        LESS_THAN = 10
    }
}