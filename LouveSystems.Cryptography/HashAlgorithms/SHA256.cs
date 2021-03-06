﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LouveSystems.Cryptography.HashAlgorithms
{
    public class SHA256 : IHashAlgorithm
    {
        public byte Length => 64;
        public string Hash(byte[] rawData)
        {
            // Create a MD5   
            using (System.Security.Cryptography.SHA256 md5Hash = System.Security.Cryptography.SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = md5Hash.ComputeHash(rawData);

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("X2"));
                }

                return builder.ToString();
            }
        }
    }
}
