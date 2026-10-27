using Godot;
using Incremental.Game.Scripts.Upgrades;

namespace Incremental.Game.Scripts.Ship;

public partial class Ship : Node2D
{
    private const float ShipFootprintMargin = 0.65f;

    private Vector2 _destination;
    private bool _hasDestination;
    private Sprite2D _shipSprite;
    private Node2D _uiRoot;
    private TextureRect _topBar;
    private TextureRect _bottomBar;
    public UpgradeManager Upgrades { get; } = new();

    [Export]
    [Upgradable(UpgradeIds.ShipSpeed, 1000f, 10)]
    public float Speed = 500f;

    [Export]
    public int FPS = 30;

    private float interval;
    private float intervalMax;

    public override void _Ready()
    {
        Upgrades.Register(this);
        _destination = GlobalPosition;
        _shipSprite = GetNode<Sprite2D>("ShipSprite");
        _uiRoot = GetNode<Node2D>("../../UI");
        _topBar = GetNode<TextureRect>("../../UI/TopBar");
        _bottomBar = GetNode<TextureRect>("../../UI/BottomBar");
        intervalMax = 1f / FPS;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventScreenTouch touch && touch.Pressed)
        {
            SetDestination(touch.Position);
        }
        else if (@event is InputEventScreenDrag drag)
        {
            SetDestination(drag.Position);
        }
        else if (@event is InputEventMouseButton mouseButton &&
                 mouseButton.ButtonIndex == MouseButton.Left &&
                 mouseButton.Pressed)
        {
            SetDestination(mouseButton.Position);
        }
        else if (@event is InputEventMouseMotion mouseMotion &&
                 (mouseMotion.ButtonMask & MouseButtonMask.Left) != 0)
        {
            SetDestination(mouseMotion.Position);
        }
    }

    public override void _Process(double delta)
    {
        interval += (float)delta;

        if (!_hasDestination)
            return;

        if (FPS == 0 || interval >= intervalMax)
        {
            float distance = GlobalPosition.DistanceTo(_destination);
            if (distance < 2f)
            {
                GlobalPosition = _destination;
                _hasDestination = false;
                return;
            }

            Vector2 direction = _destination - GlobalPosition;
            GlobalRotation = direction.Angle() + Mathf.Pi / 2;
            interval = 0f;
        }

        GlobalPosition = GlobalPosition.MoveToward(_destination, Speed * (float)delta);
    }

    private void SetDestination(Vector2 viewportPosition)
    {
        Transform2D canvasTransform = GetViewport().GetCanvasTransform();
        Vector2 worldPosition = canvasTransform.AffineInverse() * viewportPosition;
        if (IsOverUi(_uiRoot, worldPosition))
            return;

        Rect2 topBarBounds = _topBar.GetGlobalRect();
        Rect2 bottomBarBounds = _bottomBar.GetGlobalRect();
        float shipRadius = GetShipFootprintRadius();

        float minX = Mathf.Max(topBarBounds.Position.X, bottomBarBounds.Position.X) + shipRadius;
        float maxX = Mathf.Min(topBarBounds.End.X, bottomBarBounds.End.X) - shipRadius;
        float minY = topBarBounds.End.Y + shipRadius;
        float maxY = bottomBarBounds.Position.Y - shipRadius;

        _destination = new Vector2(
            ClampAxis(worldPosition.X, minX, maxX),
            ClampAxis(worldPosition.Y, minY, maxY));
        _hasDestination = true;
    }

    private static bool IsOverUi(Node node, Vector2 worldPosition)
    {
        if (node is Control control &&
            control.IsVisibleInTree() &&
            control.MouseFilter != Control.MouseFilterEnum.Ignore &&
            control.GetGlobalRect().HasPoint(worldPosition))
        {
            return true;
        }

        foreach (Node child in node.GetChildren())
        {
            if (IsOverUi(child, worldPosition))
                return true;
        }

        return false;
    }

    private float GetShipFootprintRadius()
    {
        Rect2 spriteRect = _shipSprite.GetRect();
        Vector2[] corners =
        {
            spriteRect.Position,
            new Vector2(spriteRect.End.X, spriteRect.Position.Y),
            new Vector2(spriteRect.Position.X, spriteRect.End.Y),
            spriteRect.End
        };

        float radius = 0f;
        foreach (Vector2 corner in corners)
            radius = Mathf.Max(radius, (_shipSprite.GlobalTransform * corner - GlobalPosition).Length());

        return radius * ShipFootprintMargin;
    }

    private static float ClampAxis(float value, float min, float max)
    {
        if (min > max)
            return (min + max) / 2f;

        return Mathf.Clamp(value, min, max);
    }
}
