-- MySQL dump 10.13  Distrib 8.3.0, for macos13.6 (arm64)
--
-- Host: localhost    Database: DBShareData
-- ------------------------------------------------------
-- Server version	8.0.33

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `Account`
--

DROP DATABASE IF EXISTS DBShareData;

-- Create the DataShareDB database
CREATE DATABASE DBShareData;
USE DBShareData;
DROP TABLE IF EXISTS `Account`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Account` (
  `id` int NOT NULL AUTO_INCREMENT,
  `email` varchar(255) NOT NULL,
  `password` varchar(256) NOT NULL,
  `salt` varchar(256) NOT NULL,
  `createdAt` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `email` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Account`
--

LOCK TABLES `Account` WRITE;
/*!40000 ALTER TABLE `Account` DISABLE KEYS */;
INSERT INTO `Account` VALUES (1,'test@example.com','123','123','2024-06-18 03:49:19'),(2,'string@gmail.com','4ca515b2fa4f6847f9047db2f11f89b5e3e8af6f57f5c21b5903cc2c0608fff24057d997b31beede256897f882886d4ec1fb26e104a4a97789b726915c935ea5','5216675cf12ecc7080fdfd0c3f5a5abd65b89d1015465f3a4fec0d6a4571c584129830d67f90b53f5caf56ecb3a6c15eee043a59c91fd246ce7ffa38e85408ffe781e8a73d68f36c557df3c6c12056c06c03b2bf01e6eb82fc4a24310ae5f63095fd8bbdbae6e33e602ab6826b0d6081b5b68db7660ac2688202c6356172d1c2','2024-06-18 03:50:06'),(3,'string2@gmail.com','f9e0d6bc382327482efae9410ef4f24bc953bf3eaa11a8cb687fe5fd702a0761072bac609648a943b9d5b8a54e61db55279d97d0f499c29909b2534d494728b9','f8280c3c1b3333688cd567af10b4fb579a18ecf362b3aff5cfd7011497cb873926f25e36d4cf508beae7ca29455cc5e2f75a8d6012088fea4308e28bb94c94a47986c864d57cffe828fdd83cab4fe81ee87be7edda81f50acf890f7419bdf8c14ea44dec1c996c68307b5d1b07a665e9f2c9efbbd0b96f98bf31e83d6bb50a4c','2024-06-18 03:50:11');
/*!40000 ALTER TABLE `Account` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `FileStore`
--

DROP TABLE IF EXISTS `FileStore`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `FileStore` (
  `id` int NOT NULL AUTO_INCREMENT,
  `fileName` varchar(255) NOT NULL,
  `fileData` longblob NOT NULL,
  `uploadedAt` datetime DEFAULT CURRENT_TIMESTAMP,
  `fileSize` int NOT NULL,
  `autoDelete` tinyint(1) NOT NULL,
  `Owner` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `Owner` (`Owner`),
  CONSTRAINT `filestore_ibfk_1` FOREIGN KEY (`Owner`) REFERENCES `Account` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `RefreshToken`
--

DROP TABLE IF EXISTS `RefreshToken`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `RefreshToken` (
  `id` int NOT NULL AUTO_INCREMENT,
  `token` varchar(255) NOT NULL,
  `createTime` datetime NOT NULL,
  `expiredTime` datetime DEFAULT NULL,
  `userId` int NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `token` (`token`),
  KEY `userId` (`userId`),
  CONSTRAINT `refreshtoken_ibfk_1` FOREIGN KEY (`userId`) REFERENCES `Account` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `RefreshToken`
--

LOCK TABLES `RefreshToken` WRITE;
/*!40000 ALTER TABLE `RefreshToken` DISABLE KEYS */;
INSERT INTO `RefreshToken` VALUES (1,'tiY5otq0Xv34IjaVJJN6x7LLvPWAWJQ2pq4E+GYdI80sHSZA/THYaNXNJ0UDq4dDyn+C3q4N7wihN9ZLMFl22Q==','2024-06-18 10:23:25','2024-06-25 10:23:25',2),(2,'lxao8ufh5xOjS65cInCPOreaUTk4TH5xUMKgw9zhAk9RC2xiBQ4rpkghYkGSamYWWoZdLQ1dT7o7Fe7GUnSfpw==','2024-06-18 03:50:11','2024-06-25 03:50:11',3);
/*!40000 ALTER TABLE `RefreshToken` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `TextStore`
--

DROP TABLE IF EXISTS `TextStore`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `TextStore` (
  `id` int NOT NULL AUTO_INCREMENT,
  `content` text NOT NULL,
  `createdAt` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `autoDelete` tinyint(1) NOT NULL,
  `Owner` int DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `Owner` (`Owner`),
  CONSTRAINT `textstore_ibfk_1` FOREIGN KEY (`Owner`) REFERENCES `Account` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `TextStore`
--

LOCK TABLES `TextStore` WRITE;
/*!40000 ALTER TABLE `TextStore` DISABLE KEYS */;
INSERT INTO `TextStore` VALUES (2,'Message from nowhere','2024-06-17 20:58:48',0,2);
/*!40000 ALTER TABLE `TextStore` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-06-18 23:02:10
