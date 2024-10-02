import React, { useState } from 'react';
import { S3, CreateBucketCommand, ListBucketsCommand, PutObjectCommand } from '@aws-sdk/client-s3';

const DO_SPACES_ACCESS_KEY = "DO00VYGV2L269TPLEQMV"
const DO_SPACES_SECRET_KEY = "KQ3RXicuaQNxXF/0rAnBFXnLjshLLPf5TWI4TRF7dwM"
// const DO_SPACES_BUCKET = "antidotecdn"


// const putBucketParams = {
//     Bucket: DO_SPACES_BUCKET,
//     Key: "example.txt",
//     Body: "content"
// };
// https://antidotecdn.nyc3.digitaloceanspaces.com/images/Antidote_Healix%20white.png

// https://antidotecdn.nyc3.digitaloceanspaces.com/images/Antidote_Healix%20white.png

// https://antidotecdn.nyc3.digitaloceanspaces.com

// https://antidotecdn.nyc3.cdn.digitaloceanspaces.com

const s3Client = new S3 ({
    forcePathStyle: false,
    endpoint: 'https://nyc3.digitaloceanspaces.com', // Update with your DigitalOcean Space endpoint
    region: 'us-east-1', // Update with the region of your Space
    credentials: {
        accessKeyId: DO_SPACES_ACCESS_KEY,
        secretAccessKey: DO_SPACES_SECRET_KEY,
    }
});

// const createBucketParams = { Bucket: "example-bucket-name" };

// // Creates the new Space.
// const createBucket = async () => {
// try {
//     const data = await s3client.send(new CreateBucketCommand(createBucketParams));
//     console.log("Success", data.Location);
//     return data;
// } catch (err) {
//     console.log("Error", err);
// }
// };

// createBucket();


// Returns a list of Spaces in your region.
export const listBuckts = async () => {
    try {
      const data = await s3Client.send(new ListBucketsCommand({}));
      console.log("Success", data.Buckets);
      return data; // For unit tests.
    } catch (err) {
      console.log("Error", err);
    }
  };

  listBuckts();

// const putBucket = async () => {
//     try {


//         const data = await s3client.send(new PutObjectCommand(putBucketParams));
//         console.log(
//         "Successfully uploaded object: "
//         //  +
//         //     bucketParams.Bucket +
//         //     "/" +
//         //     bucketParams.Key
//         );
//         return data;
//     } catch (err) {
//         console.log("Error", err);
//     }
// };




const DigitalOceanSpaceUpload: React.FC = () => {
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [uploading, setUploading] = useState(false);
    const [uploadProgress, setUploadProgress] = useState<number | null>(null);

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files && e.target.files[0]) {
        setSelectedFile(e.target.files[0]);
        }
    };
    // putBucket();
//   const handleUpload = async () => {
//     if (!selectedFile) return;

//     setUploading(true);
//     setUploadProgress(0);

//     const params: AWS.S3.PutObjectRequest = {
//       Bucket: 'YOUR_BUCKET_NAME',
//       Key: selectedFile.name,
//       Body: selectedFile,
//     };

//     const upload = s3.upload(params);

//     upload.on('httpUploadProgress', (progress: AWS.S3.ManagedUpload.Progress) => {
//       setUploadProgress(Math.round((progress.loaded / progress.total) * 100));
//     });

//     try {
//       const result = await upload.promise();
//       console.log('File uploaded:', result);
//     } catch (error) {
//       console.error('Error uploading file:', error);
//     } finally {
//       setUploading(false);
//       setUploadProgress(null);
//     }
//   };

  return (
    <div>
      <input type="file" onChange={handleFileChange} />
      <button>
        {/* // onClick={handleUpload} disabled={uploading}> */}
        Upload File
      </button>
      {uploading && (
        <div>
          Uploading... {uploadProgress !== null ? `${uploadProgress}%` : ''}
        </div>
      )}
    </div>
  );
};

export default DigitalOceanSpaceUpload;

// // const DigitalOceanCdn2: React.FC = () => {
// //     const [selectedFile, setSelectedFile] = useState<File | null>(null);
// //     const [uploading, setUploading] = useState(false);
// //     const [uploadProgress, setUploadProgress] = useState<number | null>(null);

// //     const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
// //         if (e.target.files && e.target.files[0]) {
// //             setSelectedFile(e.target.files[0]);
// //         }
// //     };


// //     const handleUpload = async () => {
// //         if (!selectedFile) return;

// //         setUploading(true);
// //         setUploadProgress(0);
// // //     // https://antidotecdn.nyc3.cdn.digitaloceanspaces.com/images/Antidote_Healix%20white.png
// // //     // let conn = {
// // //     //     endpoint: "nyc3.digitaloceanspaces.com",
// // //     //     accessKeyId: "", //process.env.SPACES_ACCESS_KEY,
// // //     //     secretAccessKey: ""  //process.env.SPACES_SECRET_KEY,
// // //     //     };
// //         const spacesEndpoint = new AWS.Endpoint('https://antidotecdn.nyc3.digitaloceanspaces.com/images');
// // //     const s3 = new AWS.S3({
// // //       endpoint: spacesEndpoint,
// // //       accessKeyId: DO_SPACES_ACCESS_KEY,
// // //       secretAccessKey: DO_SPACES_SECRET_KEY,
// // //     });

// // //     const params: AWS.S3.PutObjectRequest = {
// // //       Bucket: DO_SPACES_BUCKET,
// // //       Key: selectedFile.name,
// // //       Body: selectedFile,
// // //     };

// // //     const upload = s3.upload(params);

// // //     upload.on('httpUploadProgress', (progress: AWS.S3.ManagedUpload.Progress) => {
// // //       setUploadProgress(Math.round((progress.loaded / progress.total) * 100));
// // //     });

// // //     try {
// // //       const result = await upload.promise();
// // //       console.log('File uploaded:', result);
// // //     } catch (error) {
// // //       console.error('Error uploading file:', error);
// // //     } finally {
// // //       setUploading(false);
// // //       setUploadProgress(null);
// // //     }
// //     };

// //     return (
// //         <div>
// //             <input type="file" />
// //                 {/* onChange={handleFileChange} */}

// //             <button >
// //                  {/* onClick={handleUpload} disabled={uploading}> */}
// //                 Upload File
// //             </button>
// //             { false && (
// //                     <div>
// //                         Uploading...
// //                         {/* {uploadProgress !== null ? `${uploadProgress}%` : ''} */}
// //                     </div>
// //                 )
// //             }
// //         </div>
// //     )
// // }
// // export default DigitalOceanCdn2;

// // const DigitalOceanSpaceUpload: React.FC = () => {
// //   const [selectedFile, setSelectedFile] = useState<File | null>(null);
// //   const [uploading, setUploading] = useState(false);
// //   const [uploadProgress, setUploadProgress] = useState<number | null>(null);

// //   const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
// //     if (e.target.files && e.target.files[0]) {
// //       setSelectedFile(e.target.files[0]);
// //     }
// //   };

// //     const DO_SPACES_ACCESS_KEY = "DO00BYZ9FRZZXNK2MRKP"
// //     const DO_SPACES_SECRET_KEY = "+XJQiUN0XFQQC1XHueYSCZHDxaJ32qzOtahvtXw8mfo"
// //     const DO_SPACES_BUCKET = "antidotecdn"
// //   const handleUpload = async () => {
// //     if (!selectedFile) return;

// //     setUploading(true);
// //     setUploadProgress(0);
// //     // https://antidotecdn.nyc3.cdn.digitaloceanspaces.com/images/Antidote_Healix%20white.png
// //     // let conn = {
// //     //     endpoint: "nyc3.digitaloceanspaces.com",
// //     //     accessKeyId: "", //process.env.SPACES_ACCESS_KEY,
// //     //     secretAccessKey: ""  //process.env.SPACES_SECRET_KEY,
// //     //     };
// //     const spacesEndpoint = new AWS.Endpoint('https://antidotecdn.nyc3.digitaloceanspaces.com/images');
// //     const s3 = new AWS.S3({
// //       endpoint: spacesEndpoint,
// //       accessKeyId: DO_SPACES_ACCESS_KEY,
// //       secretAccessKey: DO_SPACES_SECRET_KEY,
// //     });

// //     const params: AWS.S3.PutObjectRequest = {
// //       Bucket: DO_SPACES_BUCKET,
// //       Key: selectedFile.name,
// //       Body: selectedFile,
// //     };

// //     const upload = s3.upload(params);

// //     upload.on('httpUploadProgress', (progress: AWS.S3.ManagedUpload.Progress) => {
// //       setUploadProgress(Math.round((progress.loaded / progress.total) * 100));
// //     });

// //     try {
// //       const result = await upload.promise();
// //       console.log('File uploaded:', result);
// //     } catch (error) {
// //       console.error('Error uploading file:', error);
// //     } finally {
// //       setUploading(false);
// //       setUploadProgress(null);
// //     }
// //   };

// //   return (
// //     <div>
// //       <input type="file" onChange={handleFileChange} />
// //       <button onClick={handleUpload} disabled={uploading}>
// //         Upload File
// //       </button>
// //       {uploading && (
// //         <div>
// //           Uploading... {uploadProgress !== null ? `${uploadProgress}%` : ''}
// //         </div>
// //       )}
// //     </div>
// //   );
// // };

// // export default DigitalOceanSpaceUpload;
