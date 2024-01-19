var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
// Function to fetch topic information
const getTopicInfo = () => __awaiter(this, void 0, void 0, function* () {
    try {
        const response = yield fetch('https://3piwfh0dlf.execute-api.us-east-2.amazonaws.com/GetTopicInfo');
        const responseJSON = yield response.json();
        return responseJSON;
    }
    catch (error) {
        console.error('Error fetching topic information:', error);
        throw error;
    }
});
// Function to populate topic information
function populateTopicInfo(topicInfo) {
    const template = document.getElementById('topicInfo');
    const topicInfoElement = document.importNode(template.content, true);
    // Assuming you have elements with class 'topicName' and 'author'
    topicInfoElement.querySelector('.topicName').textContent = topicInfo.topicName;
    topicInfoElement.querySelector('.author').textContent = topicInfo.topicAuthor;
    // Append the populated topicInfoElement to the body
    document.body.appendChild(topicInfoElement);
}
// Call the function
getTopicInfo()
    .then((topicInfo) => {
    document.title = topicInfo.topicName;
    populateTopicInfo(topicInfo);
})
    .catch((error) => {
    // Handle error
});
