using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

namespace RDG.UnityUtil.Scripts.Cursor {
    
    [Serializable]
    public class CursorTileSheet {
        public Texture2D texture;
        public Vector2Int tileSize;
        public Vector2Int tileSpacing;
    }

    [Serializable]
    public class CursorStateTile {
        public CursorInteractionSo interaction;
        //Tile Position in Sheet from Bottom Left indexed from 0
        public Vector2Int position;
        public bool hasDownTile;
        public Vector2Int downPosition;
        public Vector2 hotSpot;
        public Vector2 hotSpotDown;
    }
    
    [Serializable]
    public class CursorConfig {
        public CursorTileSheet sheet;
        public CursorInteractionSo defaultInteraction;
        public CursorStateTile[] tiles;
    }

    internal class CursorItem {
        public Texture2D Texture;
        public Texture2D TextureDown;
        public bool HasDown;
        public Vector2 HotSpot;
        public Vector2 HotSpotDown;
    }

    internal class CursorTicket {
        public CursorTicket(long index, CursorInteractionSo interaction) {
            Index = index;
            Interaction = interaction;
        }
        public readonly long Index;
        public readonly CursorInteractionSo Interaction;
        public readonly Action<bool> OnDownChange;
    }
    
    [CreateAssetMenu(menuName = "RDG/Util/Cursor/CursorRegistry")]
    public class CursorRegistrySo : ScriptableObject {
        
        public CursorConfig config;

        public UnityEvent<bool> onDownChange;
        
        private Dictionary<string, CursorItem> stateToItem;
        private List<CursorTicket> tickets;
        private long nextIndex;
        private CursorInteractionSo currentInteraction;
        private bool isDown;
        
        
        internal void Reset() {
            stateToItem = new Dictionary<string, CursorItem>();
            foreach (var tile in config.tiles) {
                var texture = GetTexture(tile.position);
                Texture2D textureDown = null;
                if (tile.hasDownTile) {
                    textureDown = GetTexture(tile.downPosition);
                }
                stateToItem[tile.interaction.name] = new CursorItem(){
                    Texture = texture,
                    TextureDown = textureDown,
                    HasDown = tile.hasDownTile,
                    HotSpot = tile.hotSpot,
                    HotSpotDown = tile.hotSpotDown,
                };
            }
            isDown = false;
            nextIndex = 0;
            tickets = new List<CursorTicket>();
            Push(config.defaultInteraction);
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

        public Action Push(CursorInteractionSo interaction) {
            if (tickets == null) {
                Reset();    
            }
            var ticket = new CursorTicket(nextIndex++, interaction);
            tickets?.Add(ticket);
            UpdateState(ticket.Interaction);
            return PopAction;

            void PopAction() {
                Pop(ticket);
            }
        }

        private void Pop(CursorTicket ticket) {
            var ticketIndex = tickets.FindIndex(t => t.Index == ticket.Index);
            if (ticketIndex < 0) {
                return;
            }
            
            tickets.RemoveAt(ticketIndex);
            UpdateState(tickets.Last().Interaction);
        }

        private void UpdateState(CursorInteractionSo requested) {
            var oldState = currentInteraction;
            currentInteraction = requested;
            if (currentInteraction == oldState) {
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
            if (wasDown == isDown) return;
            
            onDownChange?.Invoke(isDown);
            ApplyCursor();
        }
        
        private void ApplyCursor() {
            var item = stateToItem[currentInteraction.name];
            if (item == null) {
                UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                return;
            }
            
            var texture = (item.HasDown && isDown) ? item.TextureDown : item.Texture;
            var hotSpot = (item.HasDown && isDown) ? item.HotSpotDown : item.HotSpot;
            UnityEngine.Cursor.SetCursor(texture, hotSpot, CursorMode.Auto);   
        }
    }
}
