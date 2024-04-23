# Social_Butterfly_Experience_Scripts
Collection of Scripts I was mainly responsible for in a group project.

## Scripts and Functions
### Player
Object controlled by players. Tracks previous position using a queue of TrailPoints.

__Functions__

- update_trail()
  - Updates queue with new TrailPoint
  - Updates particle system

- check_moving()
  - Checks whether the player is moving, based on distance traveled

- realtime_movement()
  - Moves object based on an input object. Input object contains realtime data from a motion caption system.

- manual_movement()
  - Moves object using WASD controls. Used for debugging, when the motion capture system is not available.
  
### Butterfly
Butterfly gameObject that follows the player and interacts with the environment.

### Manager
Object that keeps track of all players and butterflies, providing a connection between them. Also manages gamestate.

__Functions__

- spawn_butterflies()
  - spawns player specific butterflies and communal butterflies using the spawn_butterflies_helper function

- spawn_butterflies_helper()
  - Instantiates butterfly gameObject at a random position
  - Assigns ID to butterfly, corresponding to a player

- start_game()

- get_trail_points()
  - Consolidate TrailPoints of active players. Returns a list of TrailPoints

- delete_butterfly()

- redistribute_ids()

- update_progress_bar()

- check_game_over()

- send_osc()
  - send OSC message using the OSC JACK plugin

### StartBox
Changes color if corresponding player is in contact.

### TrailPoint
Class containing a Vector3, representing position, and an ID, representing the player.
