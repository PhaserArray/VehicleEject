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
