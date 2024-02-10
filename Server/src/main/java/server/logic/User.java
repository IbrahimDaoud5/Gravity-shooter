package server.logic;

public class User {
        private String username;
        private String password;
        private boolean isReady; // Has to be atomic? users can set their own isReady, and can read the other's is ready.
        private boolean inGame;
        private String invitedBy;

    public User() {
    }
    public User(String username, String password) {
        this.username = username;
        this.password = password;
    }
    public String getUsername() {
        return username;
    }
    public void setUsername(String username) {
        this.username = username;
    }
    public String getPassword() {
        return password;
    }
    public void setPassword(String password) {
        this.password = password;
    }
    public boolean isReady() { return isReady;}
    public void setReady(boolean isReady) {  this.isReady = isReady; }
    public boolean isInGame() { return inGame;}
    public void setInGame(boolean inGame) {  this.inGame = inGame; }
    public String getInvitedBy() {
        return invitedBy;
    }

    public void setInvitedBy(String invitedBy) {
        this.invitedBy = invitedBy;
    }

    @Override
    public String toString() {
        return "User{" +
                "username='" + username + '\'' +
                ", isReady=" + isReady +
                ", inGame=" + inGame +
                '}';
    }
}
