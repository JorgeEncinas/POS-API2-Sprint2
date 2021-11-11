﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POSAPI2
{
    class DBConnection
    {
        private string connLocal = "server=127.0.0.1; user=root; database=verpres3; SSL mode=none";
        private MySqlConnection mySqlConn;

        public DBConnection()
        {
            mySqlConn = new MySqlConnection(connLocal);
        }

        public void executeNonQuery(string query)
        {
            try
            {
                mySqlConn.Open();
                MySqlCommand command = new MySqlCommand(query, mySqlConn);
                command.ExecuteNonQuery();
            } catch(MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            mySqlConn.Close();
        }

        public int executeScalar(string query)
        {
            int result = 0;
            try
            {
                mySqlConn.Open();
                MySqlCommand command = new MySqlCommand(query, mySqlConn);
                result = Convert.ToInt32(command.ExecuteScalar());
            } catch(MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            mySqlConn.Close();
            return result;
        }

        public MySqlDataReader queryProduct(string query)
        {
            MySqlDataReader reader = null;
            try
            {
                mySqlConn.Open();
                MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConn);
                reader = mySqlCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            } catch(Exception ex)
            {
                mySqlConn.Close();
                MessageBox.Show(ex.ToString());
            }
            return reader;
        }
        
        public bool compareAES(string passwd)
        {
            bool passwdCheck = false;
            using (Aes myAes = Aes.Create())
            {
                // Encrypt the string to an array of bytes.
                byte[] encrypted = EncryptStringToBytes_AES(passwd, myAes.Key, myAes.IV);
            }
                return passwdCheck;
        }
        public string encryptAES(string passwd)
        {
            string encryptedpwd = "";
            using (Aes myAes = Aes.Create())
            {
                // Encrypt the string to an array of bytes.
                byte[] encrypted = EncryptStringToBytes_AES(passwd, myAes.Key, myAes.IV);
                encryptedpwd = BitConverter.ToString(encrypted);
            }
            return encryptedpwd;
        }
        static byte[] EncryptStringToBytes_AES(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
    }
}
