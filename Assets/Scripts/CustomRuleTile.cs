using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Craft2D/ Custom Tiles/ Custom Rule Tile")]
public class CustomRuleTile : RuleTile<CustomRuleTile.Neighbor> {
    public bool customField;

    public TileBase water;
    public TileBase sand;

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Null = 3;
        public const int NotNull = 4;
        public const int Water = 5;
        public const int Sand = 6;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.Null: return tile == null;
            case Neighbor.NotNull: return tile != null;
            case Neighbor.Water: return tile == water;
            case Neighbor.Sand: return tile == sand;
        }
        return base.RuleMatch(neighbor, tile);
    }
}