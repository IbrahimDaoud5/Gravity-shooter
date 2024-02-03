package server.logic;


import org.springframework.stereotype.Service;

import java.util.Map;

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

    public String[] showConnectedUsers(String query) {
        String[] a = activeUsers.entrySet().stream()
                .filter(entry -> !entry.getValue().isInGame())
                .filter(entry -> query == null || entry.getKey().startsWith(query))
                .map(Map.Entry::getKey)
                .toArray(String[]::new);
        System.out.println(a);
        return a;
    }

    // Method to send an invitation
    public String sendInvitation(String fromUsername, String toUsername) {
        User toUser = activeUsers.get(toUsername);
        if (toUser != null && !toUser.isInGame()) {
            toUser.setInvitedBy(fromUsername);
            return "Invitation sent to " + toUsername;
        }
        return "User is not available";
    }

    // Method for a user to check their invitation
    public String checkInvitation(String username) {
        User user = null;
        System.out.println("Username is ----> "+username);
        try {
            //printActiveSessions();
            user = activeUsers.get(username);

        }
        catch (NullPointerException nullPointerException)
        {

        }


        if (user != null && user.getInvitedBy() != null && !user.getInvitedBy().isEmpty()) {
            String inviter = user.getInvitedBy();
            user.setInvitedBy(null); // Reset invitation after checking
            return inviter;
        }
        return "";
    }


}
