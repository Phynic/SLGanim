using UnityEngine;

/// <summary>
/// Human Round
/// </summary>
class HumanPlayer : Player
{
    public override void Play()
    {
        RoundManager.GetInstance().RoundState = new RoundStateWaitingForInput();
    }
}