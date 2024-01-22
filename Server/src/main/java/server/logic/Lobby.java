package server.logic;

import static server.logic.LoginService.activeUsers;

public class Lobby {
    public String start(String username) {

        return username;
    }


    public void setReadyStatus(User user) {
        if (activeUsers.containsKey(user.getUsername())) {
            activeUsers.put(user.getUsername(),user );
        }
    }


    // Method to check if a user is ready
    public boolean isUserReady(String username) {
       // return activeSessions.getOrDefault(username,null);
        return false;
    }
}
