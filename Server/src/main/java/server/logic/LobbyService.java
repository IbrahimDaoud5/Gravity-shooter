package server.logic;


import org.springframework.stereotype.Service;

import static server.logic.LoginService.activeUsers;
import static server.logic.LoginService.printActiveSessions;

@Service
public class LobbyService {
    public String start(String username) {

        return username;
    }

    // Method to handle user logout
    public synchronized String logout(String username) {
        User user = activeUsers.get(username);
        if (user != null) {
            activeUsers.remove(username);
        }
        return "logged out successfully";
    }

    public String setToReady(String username) {
        printActiveSessions();
        activeUsers.get(username).setReady(true); ////////////////////////////////// HANDLE get returning null
        return username + "'s status is set to READY";
    }
}
