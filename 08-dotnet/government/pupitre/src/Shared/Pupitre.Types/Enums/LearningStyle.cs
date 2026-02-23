namespace Pupitre.Types.Enums;

/// <summary>
/// Student learning style preferences.
/// </summary>
public enum LearningStyle
{
    /// <summary>Learns best through seeing (diagrams, videos, reading)</summary>
    Visual = 1,
    
    /// <summary>Learns best through hearing (lectures, discussions)</summary>
    Auditory = 2,
    
    /// <summary>Learns best through reading and writing</summary>
    ReadingWriting = 3,
    
    /// <summary>Learns best through hands-on activities</summary>
    Kinesthetic = 4,
    
    /// <summary>Prefers logical, systematic approach</summary>
    Logical = 5,
    
    /// <summary>Prefers learning in groups</summary>
    Social = 6,
    
    /// <summary>Prefers self-study</summary>
    Solitary = 7,
    
    /// <summary>Mixed or adaptive style</summary>
    Mixed = 8
}
