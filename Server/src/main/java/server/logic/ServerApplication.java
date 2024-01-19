package server.logic;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
/**
 * The entry point of the Spring Boot application.
 * This class is responsible for bootstrapping and launching the Spring application.
 */
@SpringBootApplication
public class ServerApplication {
    /**
     * Main method which serves as the entry point for the application.
     * It delegates to Spring Boot's SpringApplication class to bootstrap the application.
     *
     * @param args Command line arguments passed to the application.
     */
    public static void main(String[] args) {
        SpringApplication.run(ServerApplication.class, args);
    }
}
