using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TinyCacto.Utils;

namespace TinyCacto.Effects
{
    public static class TinyEffects
    {

        // TODO: Change everything that uses a routine to DOTween

        #region GHOST TRAIL EFFECT

        /// <summary>
        /// Spawn a ghost trail effect of only the sprite renderer
        /// </summary>
        /// <param name="routineHolder">Who's going to handle the routine.
        /// Don't destroy the holder script while the routine is happening.</param>
        /// <param name="interval">Interval in seconds for a new ghost to be spawned.</param>
        /// <param name="duration">How long will ghosts keep spawning.</param>
        /// <param name="fadeSpeed">How fast will the ghost disappear in a fade out effect.</param>
        public static Coroutine SpawnGhostTrail(this SpriteRenderer s, float interval, float duration, float fadeSpeed)
        {
            if (s == null)
                return null;

            if (s.gameObject == null)
                return null;

            return TinyRoutineHolder.Instance.StartCoroutine(RoutineTrail());

            IEnumerator RoutineTrail()
            {
                GameObject visualGO = s.gameObject;
                var time = 0f;
                var startTime = Time.time;
                while (Time.time - startTime < duration)
                {
                    if (time + interval < Time.time - startTime)
                    {
                        if (visualGO == null)
                        {
                            break;
                        }

                        GameObject go = Instantiate(visualGO, s.transform.position);
                        go.GetComponent<SpriteRenderer>().FadeOut(fadeSpeed, 0.25f, () => Destroy(go));

                        if (go.TryGetComponent(out Animator anim))
                        {
                            // Remove the animator
                            Destroy(anim);
                        }

                        time = Time.time - startTime;
                    }

                    yield return new WaitForSeconds(Time.deltaTime);
                }
            }

        }

        public static void StopGhostTrail(Coroutine c)
        {
            TinyRoutineHolder.Instance.StopCoroutine(c);
        }

        #endregion

        #region SET ALPHA

        public static void SetAlpha(this Image i, float newAlpha)
        {
            Color c = i.color;
            c.a = newAlpha;

            i.color = c;
        }

        public static void SetAlpha(this SpriteRenderer sr, float newAlpha)
        {
            Color c = sr.color;
            c.a = newAlpha;

            sr.color = c;
        }


        #endregion

        
        #region FADE OUT

        public static void FadeOut(this SpriteRenderer i, float duration, float onCompleteDelay, System.Action OnComplete)
        {
            TinyRoutineHolder.Instance.StartCoroutine(FadeOutRoutine());

            IEnumerator FadeOutRoutine()
            {
                float journey = duration;

                while (journey > 0)
                {
                    journey -= Time.deltaTime;
                    i.SetAlpha(journey / duration);

                    yield return new WaitForFixedUpdate();
                }

                yield return new WaitForSeconds(onCompleteDelay);

                OnComplete?.Invoke();
            }
        }

        #endregion


        #region FORCE MOVE

        public static void ForceMove(this Transform t, Vector3 newPos, float duration, System.Action OnComplete)
        {
            TinyRoutineHolder.Instance.StartCoroutine(ForceMoveRoutine());

            IEnumerator ForceMoveRoutine()
            {
                float journey = 0;
                Vector3 startPos = t.position;

                while (journey <= duration)
                {
                    journey += Time.deltaTime;

                    t.position = Vector3.Lerp(startPos, newPos, journey / duration);

                    yield return new WaitForFixedUpdate();
                }

                OnComplete?.Invoke();
            }
        }

        #endregion


        #region HELPERS

        internal static void Destroy(Object o) => Object.Destroy(o);
        internal static GameObject Instantiate(GameObject o, Vector3 v, Transform p) => Object.Instantiate(o, v, Quaternion.identity, p);
        internal static GameObject Instantiate(GameObject o, Vector3 v) => Object.Instantiate(o, v, Quaternion.identity);

        #endregion



    }
}