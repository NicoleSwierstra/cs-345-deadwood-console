/* 
 * Nicole Swierstra
 * 
 * A class that contains constants for the client and game to talk to each other. Also doubles as command documentation.
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
    ID_MOVE = 0x10, 
    /* 
     * Command to signal that a player wants to take a role. 
     *
     * Args:
     *    0: player_id
     *    1: role_id - the list of roles in order of what they are on the json file, extras first then the current scene card
     */ 
    ID_TAKE,
    /*
     * Command that signals a player wants to upgrade
     * 
     * Args: 
     *    0: player_id
     *    1: upgrade type
     *    2: new level #
     */ 
    ID_UPGRADE,
    /*
     * Command that signals that a player wants to rehearse
     *
     * Args:
     *    0: player_id
     */ 
    ID_REHEARSE, 
    /*
     * Command that signals that a player wants to rehearse
     *
     * Args:
     *    0: player_id
     *    1: roll value (this is passed in to allow like dice based physics simulations or whatever)
     */
    ID_ACT,

    /*
     * Util command that sends information about the current tile a player is on
     * 
     * Args: 
     *    0: player_id
     */
    ID_TILEINFO, 
    ID_FORCE_END
};

public enum ClientCommands {
    INVALID_INPUT = 0x20,
    ADD_REMOTE_PLAYER,
    RM_REMOTE_PLAYER,
    PLAYER_TURN,
    REVEAL_NEIGHBORS,
    END_DAY,
    END_GAME,
}

public enum UpgradeType { DOLLARS, CREDITS }