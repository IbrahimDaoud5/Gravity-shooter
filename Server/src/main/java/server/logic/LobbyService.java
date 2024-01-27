package server.logic;


import org.springframework.stereotype.Service;

@Service
public class LobbyService {
    public String start(String username) {

        return username;
    }

/*
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

 */

    // Method to handle user logout
    public synchronized String logout(String username) {
        User user = LoginService.getUser(username);
        if (user != null) {
            LoginService.removeUser(username);
        }
        return "logged out successfully";
    }

    public String isReady(String username) {
        User user = LoginService.getUser(username);
        if(user.isReady())
             return "ready";
        else return "not ready";
    }
}
