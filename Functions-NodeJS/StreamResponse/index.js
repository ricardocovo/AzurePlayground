
module.exports = async function (context, req) {
    context.log('JavaScript HTTP trigger function processed a request.');

    const name = (req.query.name || (req.body && req.body.name));
    const responseMessage = name
        ? "Hello, " + name + ". This HTTP triggered function executed successfully."
        : "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response.";

    const buffer = Buffer.from(responseMessage);
    context.res = {
        // status: 200, /* Defaults to 200 */
        headers: {
            "Content-Type": "text/plain",
            "Content-Disposition": "attachment; filename=response-sample.txt"
        },
        isRaw: true,
        body: buffer
    };
}