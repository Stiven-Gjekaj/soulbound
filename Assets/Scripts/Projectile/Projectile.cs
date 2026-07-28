using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.UI;

// The base projectile class. All projectiles, including new/combined types, should inherit from this.
public abstract class Projectile : MonoBehaviour {
    /*
     * Commented out because Z indices don't really work yet because the Unity 5 UI likes to work differently, despite operating in world space.
     *
    public const float Z_INDEX_INITIAL = 30.0f; //Z index the projectiles start spawning at, reset after every wave
    public static float Z_INDEX_NEXT { //Used to set the initial Z position for projectiles when they're created.
        get {
            zIndexCurrent -= 0.001f;
            return zIndexCurrent;
        }
        set { zIndexCurrent = value; }
    }
    private static float zIndexCurrent = Z_INDEX_INITIAL;
     */

    internal Script owner;
    protected internal RectTransform self; // RectTransform of this projectile
    protected internal ProjectileController ctrl;
    protected internal Color32[] texture;
    private static Color32[] playerHitbox;
    private Image img;
    public bool needUpdateTex = true;
    public Rect selfAbs = new Rect(-999, -999, 0, 0); // Rectangle containing position and size of this projectile

    private bool currentlyVisible = true; // Used to keep track of whether this object is currently visible to potentially save time in SetRenderingActive().
    //private bool Collision = false;

    public bool ppcollision;
    public bool ppchanged;

    public bool needSizeRefresh;
    private float internalRotation = -1;

    /// <summary>
    /// Built-in Unity function run for initialization
    /// </summary>
    private void Awake() {
        if (playerHitbox == null) {
            playerHitbox = new Color32[64];
            for (int i = 0; i < 64; i++)
                playerHitbox[i].a = 255;
        }
        self = GetComponent<RectTransform>();
        ctrl = new ProjectileController(this);
    }

    /// <summary>
    /// Every bullet currently in play. Bullets are pooled and a pooled one is inactive, so
    /// enabling and disabling is exactly the live/not-live boundary and needs no extra
    /// bookkeeping. BattleTick walks this instead of searching the scene every step.
    /// </summary>
    private static readonly List<Projectile> live = new List<Projectile>();

    /// <summary>
    /// Advances every live bullet by one step: hitbox, position bookkeeping, collision.
    ///
    /// Walked backwards because a bullet that hits the player can remove itself, and a
    /// wave's OnHit can remove others.
    /// </summary>
    public static void TickAll() {
        for (int i = live.Count - 1; i >= 0; i--) {
            if (i >= live.Count)
                continue;
            Projectile p = live[i];
            if (p)
                p.Tick();
        }
    }

    /// <summary>
    /// Built-in Unity function run on enabling this object
    /// </summary>
    private void OnEnable() {
        live.Add(this);
        self = GetComponent<RectTransform>();
        img = GetComponent<Image>();
        img.color = Color.white;
        img.rectTransform.eulerAngles = Vector3.zero;
        Vector2 half = new Vector2(0.5f, 0.5f);
        img.rectTransform.anchorMax = half;
        img.rectTransform.anchorMin = half;
        img.rectTransform.pivot = half;
        self.sizeDelta = img.sprite.rect.size;
        //selfAbs = new Rect(self.anchoredPosition.x - self.rect.width / 2, self.anchoredPosition.y - self.rect.height / 2, self.sizeDelta.x, self.sizeDelta.y);
        ppchanged = false;
        ppcollision = ProjectileController.globalPixelPerfectCollision;
        OnStart();
        if (PlayerController.instance.texture == null)
            PlayerController.instance.texture = ((Texture2D)PlayerController.instance.selfImg.mainTexture).GetPixels32();
    }

    /// <summary>
    /// Renew the attached Projectile Controller. This is done whenever this projectile is dequeued from the bullet pool.
    /// </summary>
    public void renewController() { ctrl = new ProjectileController(this); }

    public bool isPP() { return ppcollision && ppchanged || ProjectileController.globalPixelPerfectCollision && !ppchanged; }

    private void OnDisable() { live.Remove(this); }

    /// <summary>
    /// One step of this bullet. Driven by BattleTick after the wave scripts have moved it,
    /// so the collision test below sees where the bullet actually ended up this step and
    /// where the player actually ended up, rather than whichever Unity happened to run
    /// first.
    /// </summary>
    private void Tick() {
        //ctrl.UpdatePosition();
        //OnUpdate();
        if (needSizeRefresh || internalRotation != self.eulerAngles.z)
            UpdateHitRect();
        internalRotation = self.eulerAngles.z;

        float Offsetx, Offsety;
        if (self.pivot.x != 0.5f || self.pivot.y != 0.5f) {
            float Px = (0.5f - self.pivot.x) * ctrl.sprite.width * ctrl.sprite.xscale;
            float Py = (0.5f - self.pivot.y) * ctrl.sprite.height * ctrl.sprite.yscale;
            float Pdist = Mathf.Sqrt(Mathf.Pow(Px, 2) + Mathf.Pow(Py, 2));
            float Pang = Mathf.Atan2(Py, Px) + Mathf.Deg2Rad * ctrl.sprite.rotation;
            float Centerx = Pdist * Mathf.Cos(Pang);
            float Centery = Pdist * Mathf.Sin(Pang);
            Offsetx = Centerx - selfAbs.width / 2;
            Offsety = Centery - selfAbs.height / 2;
        } else {
            Offsetx = -0.5f * selfAbs.width;
            Offsety = -0.5f * selfAbs.height;
        }

        selfAbs.x = self.position.x + Offsetx;
        selfAbs.y = self.position.y + Offsety;

        if (!HitTest()) return;
        if (isPP()) {
            if (HitTestPP())
                OnProjectileHit();
        } else
            OnProjectileHit();
    }

    /// <summary>
    /// Overrideable start function to set projectile-specific settings.
    /// </summary>
    public virtual void OnStart() { }

    /// <summary>
    /// Overrideable update function to execute on every frame.
    /// </summary>
    public virtual void OnUpdate() { }

    /// <summary>
    /// Overrideable method that executes when player is hit. Usually, this calls Hurt() on the player in some way.
    /// </summary>
    public virtual void OnProjectileHit() { PlayerController.instance.Hurt(); }

    /// <summary>
    /// Updates the projectile's hitbox.
    /// </summary>
    public virtual void UpdateHitRect() {
        if (ppcollision && ppchanged || ProjectileController.globalPixelPerfectCollision && !ppchanged) {
            float cst = self.eulerAngles.z * Mathf.Deg2Rad;
            selfAbs.width = Mathf.CeilToInt(self.sizeDelta.x * Mathf.Abs(Mathf.Cos(cst)) + self.sizeDelta.y * Mathf.Abs(Mathf.Sin(cst)));
            selfAbs.height = Mathf.CeilToInt(self.sizeDelta.y * Mathf.Abs(Mathf.Cos(cst)) + self.sizeDelta.x * Mathf.Abs(Mathf.Sin(cst)));
        } else {
            selfAbs.width = self.sizeDelta.x;
            selfAbs.height = self.sizeDelta.y;
            needSizeRefresh = false;
        }
    }

    /// <summary>
    /// Return the rectangle surrounding this projectile.
    /// </summary>
    /// <returns>The rectangle surrounding this projectile.</returns>
    public Rect getRekt() { return selfAbs; }

    /// <summary>
    /// Enable or disable rendering of this projectile and its children.
    /// </summary>
    /// <param name="active">true to enable rendering, false to disable</param>
    protected void SetRenderingActive(bool active) {
        // dont cycle through children if they arent changing state anyway
        if (currentlyVisible == active)
            return;
        if (img != null)
            img.enabled = active;
        Image[] images = GetComponentsInChildren<Image>();
        foreach (Image image in images)
            image.enabled = active;
        currentlyVisible = active;
    }

    /// <summary>
    /// Overrideable method run on every frame that should update the hitbox and return true if this projectile is hitting the player.
    /// </summary>
    /// <returns>true if there's a collision, otherwise false</returns>
    public virtual bool HitTest() {
        return selfAbs.Overlaps(PlayerController.instance.playerAbs);
    }

    /// <summary>
    /// Function that replaces the old Sprite Collision system by a Pixel-Perfect Collision system.
    /// </summary>
    /// <returns>true if there's a collision, otherwise false</returns>
    public bool HitTestPP() {
        if (!selfAbs.Overlaps(PlayerController.instance.playerAbs)) return false;
        // One texture is cached and re-read whenever the sprite changes. GetPixels32
        // allocates the whole sprite as a colour array, so an animated bullet pays that on
        // every frame it changes, and a wave full of them pays it every frame.
        //
        // Deferred to v0.8 rather than fixed: bullets are single-frame placeholders until
        // art exists, so nothing triggers it yet and there is no way to measure whether the
        // cure is worth it. Caching one texture per animation frame is the shape of the fix.
        // Recorded in docs/project/milestones.md under v0.8.
        if (needUpdateTex) {
            texture = ((Texture2D)img.mainTexture).GetPixels32();
            needUpdateTex = false;
        }

        Vector2 positionPlayerFromProjectile = (Vector2)PlayerController.instance.self.position - selfAbs.position - (selfAbs.size + PlayerController.instance.playerAbs.size) / 2;
        return UnitaleUtil.TestPP(playerHitbox, texture, ctrl.sprite.rotation, 8, img.mainTexture.height, new Vector2(ctrl.sprite.xscale, ctrl.sprite.yscale), positionPlayerFromProjectile);
    }
}