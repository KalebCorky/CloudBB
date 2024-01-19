    // Define types
    interface TopicInfo {
      topicID: number;
      topicName: string;
      topicAuthor: string;
    }

    // Function to fetch topic information
    const getTopicInfo = async (): Promise<TopicInfo> => {
      try {
        const response = await fetch('https://3piwfh0dlf.execute-api.us-east-2.amazonaws.com/GetTopicInfo');
        const responseJSON: TopicInfo = await response.json();
        return responseJSON;
      } catch (error) {
        console.error('Error fetching topic information:', error);
        throw error;
      }
    }

    // Function to populate topic information
    function populateTopicInfo(topicInfo: TopicInfo) {
      const template = document.getElementById('topicInfo') as HTMLTemplateElement;
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