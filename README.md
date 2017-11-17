# VehicleEject  
A rocket plugin that adds a command to eject other players from the vehicle.

---

### Usage: 

Command: `/eject`  
Options: `[all | others | steamid | player | seat#]`  
`all` - Ejects all passengers from the vehicle, including the caller. `a` can be used instead of `all`.  
`others`- Ejects all passengers other than the caller from the vehicle. `other` or `o` can be used instead of `others`.  
`steamid` - Ejects any passenger with the given steamid from the vehicle.  
`player` - Ejects any passenger with the given name from the vehicle.  
`seat#` - Ejects any passenger currently in the given seat.

---

### Configuration:

`LastToLockCanEject` - Whether the last player that locked the vehicle can eject passengers.  
`DriverCanEject` - Whether the driver can eject passengers.  
`AdminsCanEject` - Whether admins can eject passengers.  

---

### Permissions

`vehicle.eject` - Allows a player to call the /eject command.  
`vehicle.canalwayseject` - Allows the caller to eject passengers even if they do not meet the requirements in the config.

---

### Translations

`eject_error_notinvehicle` - Shown if the player calling the command is not in a vehicle.  
`eject_error_notdriver` - Shown if the player needs to be the driver but isn't.  
`eject_error_notlasttolock` - Shown if the player needs to be the last to lock the vehicle but isn't.  
`eject_error_noteither` - Shown if the player needs to be the driver or the last to lock the vehicle but isn't either.  
`eject_success_all` - Shown if to the caller ejects all passengers.  
`eject_success_allone`- Shown if the caller ejects all passengers but only the caller was in the vehicle.  
`eject_success_allother` - Shown if to the caller ejects all other passengers.  
`eject_error_noother` - Shown if there are no other passengers.  
`eject_success_seat` - Shown if the caller ejects a player from a seat. `{0}` is replaced with the player name `{1}`  is replaced with the seat number. `{0}` gets replaced with `eject_self` if the player ejects themselves.  
`eject_error_notaseat` - Shown if the number given does not correspond to a seat. `{0}` is replaced with the seat number.  
`eject_error_emptyseat` - Shown if the seat given is empty. `{0}` is replaced with the seat number.  
`eject_success_player` - Shown if the caller ejects a player. `{0}` is replaced with the player name or `eject_self` if the player ejects themselves.  
`eject_error_targetnotpassenger` - Shown if the player given is not a passenger. `{0}` is replaced with the player's name.  
`eject_self` - Replacement for username if the caller ejects themselves.  
`eject_notice` - Message shown to the people that get ejected. {0} gets replaced with the name of the caller. Does not get shown to the caller.
