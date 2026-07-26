using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FlyleafLib.MediaPlayer;
using LLPlayer.Extensions;

namespace LLPlayer.Services;

public class PlaybackControlsService : Bindable
{
    private readonly FlyleafManager _fl;
    private long? _lastPauseSubtitleEndTime;

    public long? LoopPointA { get; set => Set(ref field, value); }
    public long? LoopPointB { get; set => Set(ref field, value); }
    public bool IsAbLoopEnabled { get; set => Set(ref field, value); }

    public bool IsShadowingModeEnabled { get; set => Set(ref field, value); }

    public bool IsSmartSpeedEnabled { get; set => Set(ref field, value); }
    public double SmartSpeedRate { get; set => Set(ref field, value); } = 1.8;

    public HashSet<long> BookmarkedSubtitleStartTimes { get; } = new();

    public PlaybackControlsService(FlyleafManager fl)
    {
        _fl = fl;
        _fl.Player.PropertyChanged += Player_PropertyChanged;
    }

    public void ToggleAbLoopA()
    {
        LoopPointA = _fl.Player.CurTime;
        if (LoopPointB.HasValue && LoopPointB.Value <= LoopPointA.Value)
        {
            LoopPointB = null;
            IsAbLoopEnabled = false;
        }
    }

    public void ToggleAbLoopB()
    {
        if (!LoopPointA.HasValue)
        {
            LoopPointA = 0;
        }
        LoopPointB = _fl.Player.CurTime;
        if (LoopPointB.Value > LoopPointA.Value)
        {
            IsAbLoopEnabled = true;
        }
    }

    public void ClearAbLoop()
    {
        LoopPointA = null;
        LoopPointB = null;
        IsAbLoopEnabled = false;
    }

    public void ToggleBookmark(long startTimeTicks)
    {
        if (BookmarkedSubtitleStartTimes.Contains(startTimeTicks))
        {
            BookmarkedSubtitleStartTimes.Remove(startTimeTicks);
        }
        else
        {
            BookmarkedSubtitleStartTimes.Add(startTimeTicks);
        }
        OnPropertyChanged(nameof(BookmarkedSubtitleStartTimes));
    }

    public bool IsBookmarked(long startTimeTicks)
    {
        return BookmarkedSubtitleStartTimes.Contains(startTimeTicks);
    }

    private void Player_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Player.CurTime)) return;

        long curTime = _fl.Player.CurTime;

        // 1. AB Loop Handling
        if (IsAbLoopEnabled && LoopPointA.HasValue && LoopPointB.HasValue)
        {
            if (curTime >= LoopPointB.Value || curTime < LoopPointA.Value)
            {
                _fl.Player.CurTime = LoopPointA.Value;
                return;
            }
        }

        // Check subtitles for Shadowing and SmartSpeed
        var subManager = _fl.Player.SubtitlesManager[0];
        if (subManager?.Subtitles == null || subManager.Subtitles.Count == 0) return;

        var curSub = subManager.Subtitles.FirstOrDefault(s => curTime >= s.StartTime.Ticks && curTime <= s.EndTime.Ticks);

        // 2. Shadowing Auto-Pause
        if (IsShadowingModeEnabled && _fl.Player.Status == Status.Playing)
        {
            var prevSub = subManager.Subtitles.LastOrDefault(s => s.EndTime.Ticks <= curTime);
            if (prevSub != null)
            {
                long endTime = prevSub.EndTime.Ticks;
                // If we just passed subtitle end time within 200ms
                if (curTime >= endTime && curTime <= endTime + 2000000L && _lastPauseSubtitleEndTime != endTime)
                {
                    _lastPauseSubtitleEndTime = endTime;
                    _fl.Player.Pause();
                }
            }
        }

        // 3. Smart Speed (Accelerate on blank video segments)
        if (IsSmartSpeedEnabled && _fl.Player.Status == Status.Playing)
        {
            double targetSpeed = (curSub != null) ? 1.0 : SmartSpeedRate;
            if (Math.Abs(_fl.Player.Speed - targetSpeed) > 0.05)
            {
                _fl.Player.Speed = targetSpeed;
            }
        }
    }
}
