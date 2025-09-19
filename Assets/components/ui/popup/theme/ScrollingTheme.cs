using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// LimitedElasticScrollRect
/// - Kế thừa ScrollRect
/// - Giới hạn số pixel overscroll tối đa (maxOverScroll) trên mỗi chiều
/// - Yêu cầu: content pivot typical (vertical: pivot.y = 1, horizontal: pivot.x = 0)
/// - Dùng MovementType = Elastic để có bounce/elastic behavior.
/// </summary>
[AddComponentMenu("UI/Limited Elastic ScrollRect")]
public class LimitedElasticScrollRect : ScrollRect
{
    [Tooltip("Max overscroll in pixels (vertical). When pulling beyond content bounds, user can only pull up to this many pixels.)")]
    public float maxVerticalOverscroll = 10f;

    [Tooltip("Max overscroll in pixels (horizontal).")]
    public float maxHorizontalOverscroll = 0f;

    protected override void SetContentAnchoredPosition(Vector2 position)
    {
        if (content == null || viewport == null)
        {
            base.SetContentAnchoredPosition(position);
            return;
        }

        Vector2 clamped = position;

        // Vertical (assumes usual setup: content pivot.y = 1 -> anchoredPosition.y ranges [0 .. maxScroll])
        if (vertical)
        {
            float contentH = content.rect.height;
            float viewH = viewport.rect.height;
            float maxScrollY = Mathf.Max(0f, contentH - viewH); // amount of scrollable area in pixels

            // assume content pivot top (1): anchoredPosition.y nominal range is [0 .. maxScrollY]
            // allow overscroll up to maxVerticalOverscroll
            float minY = -maxVerticalOverscroll;
            float maxY = maxScrollY + maxVerticalOverscroll;

            clamped.y = Mathf.Clamp(position.y, minY, maxY);
        }

        // Horizontal (assumes content pivot.x = 0 -> anchoredPosition.x nominal range [0 .. maxScrollX])
        if (horizontal)
        {
            float contentW = content.rect.width;
            float viewW = viewport.rect.width;
            float maxScrollX = Mathf.Max(0f, contentW - viewW);

            float minX = -maxHorizontalOverscroll;
            float maxX = maxScrollX + maxHorizontalOverscroll;

            clamped.x = Mathf.Clamp(position.x, minX, maxX);
        }

        base.SetContentAnchoredPosition(clamped);
    }
}
