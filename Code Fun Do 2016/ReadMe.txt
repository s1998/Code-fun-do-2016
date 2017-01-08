Purpose of app: In events like mairrage or picnic there are too many photos clicked. So this app can help in finding all photos of a paticular person. There would be 2 inputs. First input is an image that contains only one face. Second input is a set of images. All the images in second set that  contain that face would get saved to a folder called resultImages.















Note :
1. JPEG, PNG, GIF(the first frame), and BMP are supported. The image file size should be larger than or equal to 1KB but no larger than 4MB.
2. The detectable face size is between 36x36 to 4096x4096 pixels. The faces out of this range will not be detected.
3. A maximum of 64 faces could be returned for an image. The returned faces are ranked by face rectangle size in descending order.
4. Some faces may not be detected for technical challenges, e.g. very large face angles (head-pose) or large occlusion. Frontal and near-frontal faces have the best results.





Using installer : 
Installer will help in setting up the application. After the finish of installation, Face Finder shortcut would be made on the desktop.(For wpf application)
For UWP : Open the folder and run the Add-AppDevPackage in power shell. This will install the app.













Using app :
On running app, a button called "select main image" would be visible.
Click on it, a dialog box will appear. It will help in finding the main image.The app will detect if the image is valid (has exactly one face) or not. If the image is valid, a button " select another set of images " would appear. 
Click on it, and it would help you in choosing other images.
After that a button called "match" would appear.
Click it, and app will start finding all images that contain the face in main image. All the faces found would be saved in the folder resultImages.
Click "close" to exit.






API used:
This app is based on FACE-API fom microsoft cognitive sevices.
The app uses FACE-DETECT and FACE-VERIFY. 
FACE-DETECT returns all the detected faces within an image. This data also contains the face-id of each face.
Then face-id of two faces are sent to FACE-VERIFY. It tells us if two faces belong to same person or not.

About app:
This app was created in visual studio 2015.

