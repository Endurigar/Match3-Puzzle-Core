using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
#if UNITY_EDITOR
using Screen = UnityEngine.Device.Screen; // To support Device Simulator on Unity 2021.1+
#endif

namespace IngameDebugConsole
{
	public class DebugLogPopup : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		private RectTransform popupTransform;

		private Vector2 halfSize;

		private Image backgroundImage;

		private CanvasGroup canvasGroup;

		[SerializeField]
		private DebugLogManager debugManager;

		[SerializeField]
		private TextMeshProUGUI newInfoCountText;
		[SerializeField]
		private TextMeshProUGUI newWarningCountText;
		[SerializeField]
		private TextMeshProUGUI newErrorCountText;

		[SerializeField]
		private Color alertColorInfo;
		[SerializeField]
		private Color alertColorWarning;
		[SerializeField]
		private Color alertColorError;

		private int newInfoCount = 0, newWarningCount = 0, newErrorCount = 0;

		private Color normalColor;

		private bool isPopupBeingDragged = false;
		private Vector2 normalizedPosition;

		private IEnumerator moveToPosCoroutine = null;

		public bool IsVisible { get; private set; }

        private Vector2 SavedNormalizedPosition
        {
            get => PlayerPrefs.HasKey("IDGPPos") ? JsonUtility.FromJson<Vector2>(PlayerPrefs.GetString("IDGPPos", "{}")) : new Vector2(0.5f, 0f); // Right edge by default
            set => PlayerPrefs.SetString("IDGPPos", JsonUtility.ToJson(value));
        }

		private void Awake()
		{
			popupTransform = (RectTransform) transform;
			backgroundImage = GetComponent<Image>();
			canvasGroup = GetComponent<CanvasGroup>();

			normalColor = backgroundImage.color;

			halfSize = popupTransform.sizeDelta * 0.5f;
            normalizedPosition = SavedNormalizedPosition;
		}

        protected void OnDestroy()
        {
            SavedNormalizedPosition = normalizedPosition;
        }

		public void NewLogsArrived( int newInfo, int newWarning, int newError )
		{
			if( newInfo > 0 )
			{
				newInfoCount += newInfo;
				newInfoCountText.text = newInfoCount.ToString();
			}

			if( newWarning > 0 )
			{
				newWarningCount += newWarning;
				newWarningCountText.text = newWarningCount.ToString();
			}

			if( newError > 0 )
			{
				newErrorCount += newError;
				newErrorCountText.text = newErrorCount.ToString();
			}

			if( newErrorCount > 0 )
				backgroundImage.color = alertColorError;
			else if( newWarningCount > 0 )
				backgroundImage.color = alertColorWarning;
			else
				backgroundImage.color = alertColorInfo;
		}

		private void ResetValues()
		{
			newInfoCount = 0;
			newWarningCount = 0;
			newErrorCount = 0;

			newInfoCountText.text = "0";
			newWarningCountText.text = "0";
			newErrorCountText.text = "0";

			backgroundImage.color = normalColor;
		}

		private IEnumerator MoveToPosAnimation( Vector2 targetPos )
		{
			float modifier = 0f;
			Vector2 initialPos = popupTransform.anchoredPosition;

			while( modifier < 1f )
			{
				modifier += 4f * Time.unscaledDeltaTime;
				popupTransform.anchoredPosition = Vector2.Lerp( initialPos, targetPos, modifier );

				yield return null;
			}
		}

		public void OnPointerClick( PointerEventData data )
		{
			if( !isPopupBeingDragged )
				debugManager.ShowLogWindow();
		}

		public void Show()
		{
			canvasGroup.blocksRaycasts = true;
			canvasGroup.alpha = debugManager.popupOpacity;
			IsVisible = true;

			ResetValues();

			UpdatePosition( true );
		}

		public void Hide()
		{
			canvasGroup.blocksRaycasts = false;
			canvasGroup.alpha = 0f;

			IsVisible = false;
			isPopupBeingDragged = false;
		}

		public void OnBeginDrag( PointerEventData data )
		{
			isPopupBeingDragged = true;

			if( moveToPosCoroutine != null )
			{
				StopCoroutine( moveToPosCoroutine );
				moveToPosCoroutine = null;
			}
		}

		public void OnDrag( PointerEventData data )
		{
			Vector2 localPoint;
			if( RectTransformUtility.ScreenPointToLocalPointInRectangle( debugManager.canvasTR, data.position, data.pressEventCamera, out localPoint ) )
				popupTransform.anchoredPosition = localPoint;
		}

		public void OnEndDrag( PointerEventData data )
		{
			isPopupBeingDragged = false;
			UpdatePosition( false );
		}

		public void UpdatePosition( bool immediately )
		{
			Vector2 canvasRawSize = debugManager.canvasTR.rect.size;

			float canvasWidth = canvasRawSize.x;
			float canvasHeight = canvasRawSize.y;

			float canvasBottomLeftX = 0f;
			float canvasBottomLeftY = 0f;

			if( debugManager.popupAvoidsScreenCutout )
			{
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
				Rect safeArea = Screen.safeArea;

				int screenWidth = Screen.width;
				int screenHeight = Screen.height;

				canvasWidth *= safeArea.width / screenWidth;
				canvasHeight *= safeArea.height / screenHeight;

				canvasBottomLeftX = canvasRawSize.x * ( safeArea.x / screenWidth );
				canvasBottomLeftY = canvasRawSize.y * ( safeArea.y / screenHeight );
#endif
			}

			Vector2 pos = canvasRawSize * 0.5f + ( immediately ? new Vector2( normalizedPosition.x * canvasWidth, normalizedPosition.y * canvasHeight ) : ( popupTransform.anchoredPosition - new Vector2( canvasBottomLeftX, canvasBottomLeftY ) ) );

			float distToLeft = pos.x;
			float distToRight = canvasWidth - distToLeft;

			float distToBottom = pos.y;
			float distToTop = canvasHeight - distToBottom;

			float horDistance = Mathf.Min( distToLeft, distToRight );
			float vertDistance = Mathf.Min( distToBottom, distToTop );

			if( horDistance < vertDistance )
			{
				if( distToLeft < distToRight )
					pos = new Vector2( halfSize.x, pos.y );
				else
					pos = new Vector2( canvasWidth - halfSize.x, pos.y );

				pos.y = Mathf.Clamp( pos.y, halfSize.y, canvasHeight - halfSize.y );
			}
			else
			{
				if( distToBottom < distToTop )
					pos = new Vector2( pos.x, halfSize.y );
				else
					pos = new Vector2( pos.x, canvasHeight - halfSize.y );

				pos.x = Mathf.Clamp( pos.x, halfSize.x, canvasWidth - halfSize.x );
			}

			pos -= canvasRawSize * 0.5f;

			normalizedPosition.Set( pos.x / canvasWidth, pos.y / canvasHeight );

			pos += new Vector2( canvasBottomLeftX, canvasBottomLeftY );

			if( moveToPosCoroutine != null )
			{
				StopCoroutine( moveToPosCoroutine );
				moveToPosCoroutine = null;
			}

			if( immediately )
				popupTransform.anchoredPosition = pos;
			else
			{
				moveToPosCoroutine = MoveToPosAnimation( pos );
				StartCoroutine( moveToPosCoroutine );
			}
		}
	}
}