using UnityEngine;
/// <summary>
/// Buff represents an "upgrade" to a unit.
/// </summary>
public interface IBuff
{
    /// <summary>
    /// Determines how long the buff should last (expressed in turns). If set to negative number, buff will be permanent.
    /// </summary>
    /// 负数表示。。。手动删除。
    int Duration { get; set; }


    //if(duration == 0)
    //{
    //    Duration = duration;
    //}
    //else
    //{
    //    Duration = RoundManager.GetInstance().Players.Count* duration - 1;
    //}


    /// <summary>
    /// Describes how the unit should be upgraded.
    /// </summary>
    void Apply(Transform character);
    /// <summary>
    /// Returns units stats to normal.
    /// </summary>
    void Undo(Transform character);

    /// <summary>
    /// Returns deep copy of the object.
    /// </summary>
    IBuff Clone();
}