using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RDG.UnityUtil {

    public enum CursorState {
        Default, Interact, InteractVariant1, InteractVariant2,
    }
    
    [Serializable]
    public class CursorTileSheet {
        public Texture2D texture;
        public Vector2Int tileSize;
        public Vector2Int tileSpacing;
    }

    [Serializable]
    public class CursorStateTile {
        public string name;
        //Tile Position in Sheet from Bottom Left indexed from 0
        public Vector2Int position;
        public CursorState state;
        public bool hasDownTile;
        public Vector2Int downPosition;
    }
    
    [Serializable]
    public class CursorConfig {
        public CursorTileSheet sheet;
        public CursorStateTile[] tiles;
    }

    internal class CursorItem {
        public Texture2D Texture;
        public Texture2D TextureDown;
        public bool HasDown;
    }

    internal class CursorTicket {
        public CursorTicket(long index, CursorState state) {
            Index = index;
            State = state;
        }
        public readonly long Index;
        public readonly CursorState State;
    }
    
    [CreateAssetMenu(menuName = "RDG/Util/Cursor")]
    public class CursorSo : ScriptableObject {
        
        public CursorConfig config;

        private Dictionary<CursorState, CursorItem> stateToItem;
        private List<CursorTicket> tickets;
        private long nextIndex;
        private CursorState currentState;
        private bool isDown;
        
        internal void Reset() {
            stateToItem = new Dictionary<CursorState, CursorItem>();
            foreach (var tile in config.tiles) {
                var texture = GetTexture(tile.position);
                Texture2D textureDown = null;
                if (tile.hasDownTile) {
                    textureDown = GetTexture(tile.downPosition);
                }
                stateToItem[tile.state] = new CursorItem(){
                    Texture = texture,
                    TextureDown = textureDown,
                    HasDown = tile.hasDownTile,
                };
            }
            isDown = false;
            nextIndex = 0;
            tickets = new List<CursorTicket>();
            Push(CursorState.Default);
        }

        private Texture2D GetTexture(Vector2Int position) {
            var start = ScaleVectors(position, config.sheet.tileSize + config.sheet.tileSpacing);
            var dest = new Texture2D(config.sheet.tileSize.x, config.sheet.tileSize.y, TextureFormat.RGBA32, false);
            Graphics.CopyTexture(config.sheet.texture, 0, 0, start.x, start.y, config.sheet.tileSize.x, config.sheet.tileSize.y, dest, 0, 0, 0, 0);
            return dest;
        }
        
        private static Vector2Int ScaleVectors(Vector2Int first, Vector2Int second) {
            first.Scale(second);
            return first;
        }

        public Action Push(CursorState state) {
            if (tickets == null) {
                Reset();    
            }
            var ticket = new CursorTicket(nextIndex++, state);
            tickets.Add(ticket);
            void PopAction() {
                Pop(ticket);
            }
            UpdateState(ticket.State);
            return PopAction;
        }

        private void Pop(CursorTicket ticket) {
            var ticketIndex = tickets.FindIndex(t => t.Index == ticket.Index);
            if (ticketIndex < 0) {
                return;
            }
            
            tickets.RemoveAt(ticketIndex);
            UpdateState(tickets.Last().State);
        }

        private void UpdateState(CursorState requested) {
            var oldState = currentState;
            currentState = requested;
            if (currentState == oldState) {
                return;
            }
            
            ApplyCursor();
        }

        internal void HandleUp() {
            UpdateDown(false);
        }

        internal void HandleDown() {
            UpdateDown(true);
        }

        private void UpdateDown(bool requested) {
            var wasDown = isDown;
            isDown = requested;
            if (wasDown == isDown) {
                return;
            }

            ApplyCursor();
        }
        
        private void ApplyCursor() {
            var item = stateToItem[currentState];
            if (item == null) {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                return;
            }
            
            var texture = (item.HasDown && isDown) ? item.TextureDown : item.Texture;
            Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);   
        }
    }
}
