/* 
 * Nicole Swierstra
 * 
 * A class that contains constants for the client and game to talk to each other. Also doubles as command documentation.
 * I call this programming paradigm "network oriented programming"
 */

namespace Deadwood;

public enum GameActions {
    /* 
     * Command to signal that a player wants to move. 
     *
     * Args:
     *    0: player_id
     *    1: location to move
     */ 
    MOVE = 0x10, 
    
    /* 
     * Command to signal that a player wants to take a role. 
     *
     * Args:
     *    0: player_id
     *    1: role_id - the list of roles in order of what they are on the json file, extras first then the current scene card
     */ 
    TAKE,
    
    /*
     * Command that signals a player wants to upgrade
     * 
     * Args: 
     *    0: player_id
     *    1: upgrade type
     *    2: new level #
     */ 
    UPGRADE,
    
    /*
     * Command that signals that a player wants to rehearse
     *
     * Args:
     *    0: player_id
     */ 
    REHEARSE, 
    
    /*
     * Command that signals that a player wants to rehearse
     *
     * Args:
     *    0: player_id
     *    1: roll value (this is passed in to allow like dice based physics simulations or whatever)
     */
    ACT,

    /*
     * Util command that sends information about the current tile a player is on
     * 
     * Args: 
     *    0: player_id
     */
    TILEINFO,

    /*
     * Util command to get the card info of the tile the player is on. 
     * 
     * Args:
     *    0: player_id
     */
    CARD_INFO,

    /*
     * Util command that forces the game to end.
     *
     * Args: None
     */
    FORCE_END
};

public enum ClientCommands {
   
    /*
     * Adds a remote player to the UI. This is sent between all nodes when a new player is added, and all current players are sent with this command to a new node.
     * 
     * Args:
     *    0: color
     * 1..n: packed string of player's name.
     */
    ADD_REMOTE_PLAYER = 0x30,
    
    /*
     * Removes a remote player
     *
     * Args:
     *    0: remote player_id
     */
    RM_REMOTE_PLAYER,
    
    /*
     * Begins a player's turn and prompts the UI to display the turn UI
     *
     * Args:
     *    0: player_id
     */
    PLAYER_TURN,
    
    /*
     * Sent by game board to generate the player's move UI.
     *
     * Args:
     *    0: current tile_id
     * 1..n: neighbor tile_id
     */
    REVEAL_NEIGHBORS,

    /*
     * Reveals a card from a tile.
     * 
     * Args:
     *    0: tile_id
     *    1: card_id
     */
    REVEAL_CARD,

    /*
     * Change player location
     *
     * Args:
     *    0: player_id
     *    1: new location
     */
    UPDATE_LOCATION,

    /*
     * Change player currency
     *
     * Args:
     *    0: player_id
     *    1: player's dollars
     *    2: player's credits
     *    3: player's rehersal tokens
     */
    UPDATE_CURRENCY,
    
    /*
     * Ends the current day.
     */
    END_DAY,

    /*
     * Ends the game
     * 
     * Args:
     * 0..n: player's ending scores, in order of their player id's 
     */
    END_GAME,
}

public enum UpgradeType { DOLLARS, CREDITS }