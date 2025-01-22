namespace ForthMachine;

/// <summary>
/// Syntactic scope.
/// </summary>
public enum SyntacticScope
{
    /// <summary>
    /// Root scope.
    /// </summary>
    Root = 0,

    /// <summary>
    /// Inside IF.
    /// </summary>
    If,

    /// <summary>
    /// Inside ELSE.
    /// </summary>
    Else,

    /// <summary>
    /// Inside DO loop.
    /// </summary>
    Do,

    /// <summary>
    /// Inside BEGIN loop.
    /// </summary>
    Begin,

    /// <summary>
    /// Inside a word definition.
    /// </summary>
    Word
}